using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Api.Services.Reviews;

/// <summary>
/// Service som hanterar affärslogik för recensioner och betyg.
/// </summary>
public class ReviewService : IReviewService
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Skapar en ny instans av servicen och injicerar databaskontexten.
  /// </summary>
  public ReviewService(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Skapar en ny recension för en film från den aktuella användaren.
  /// </summary>
  public async Task<ReviewDto?> CreateReviewAsync(int userId, CreateReviewDto dto)
  {
    var movie = await _context.Movies
        .FirstOrDefaultAsync(m => m.Id == dto.MovieId);

    if (movie == null)
      return null;

    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null)
      return null;

    var existingReview = await _context.Reviews
        .FirstOrDefaultAsync(r => r.MovieId == dto.MovieId && r.UserId == userId && !r.IsDeleted);

    if (existingReview != null)
    {
      throw new ArgumentException("Du har redan recenserat den här filmen.");
    }

    var review = new Review
    {
      MovieId = dto.MovieId,
      UserId = userId,
      Comment = dto.Comment,
      Rating = dto.Rating,
      CreatedAt = DateTime.UtcNow,
      UseNickname = true
    };

    _context.Reviews.Add(review);
    await _context.SaveChangesAsync();

    await UpdateMovieRatingAsync(review.MovieId);

    return MapToReviewDto(review, user);
  }

  /// <summary>
  /// Uppdaterar en befintlig recension för den aktuella användaren.
  /// </summary>
  public async Task<ReviewDto?> UpdateReviewAsync(int userId, int reviewId, UpdateReviewDto dto)
  {
    var review = await _context.Reviews
        .Include(r => r.User)
        .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId && !r.IsDeleted);

    if (review == null)
      return null;

    review.Comment = dto.Comment;
    review.Rating = dto.Rating;
    review.IsEdited = true;
    review.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    await UpdateMovieRatingAsync(review.MovieId);

    return MapToReviewDto(review, review.User);
  }

  /// <summary>
  /// Tar bort den aktuella användarens recension/kommentar.
  /// </summary>
  public async Task<bool> RemoveReviewCommentAsync(int userId, int reviewId)
  {
    var review = await _context.Reviews
        .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId && !r.IsDeleted);

    if (review == null)
      return false;

    review.Comment = null;
    review.IsEdited = true;
    review.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return true;
  }

  /// <summary>
  /// Räknar om en films genomsnittsbetyg och antal betyg baserat på aktiva recensioner.
  /// </summary>
  private async Task UpdateMovieRatingAsync(int movieId)
  {
    var movie = await _context.Movies
        .FirstOrDefaultAsync(m => m.Id == movieId);

    if (movie == null)
      return;

    var activeReviews = await _context.Reviews
        .Where(r => r.MovieId == movieId && !r.IsDeleted)
        .ToListAsync();

    movie.RatingCount = activeReviews.Count;
    movie.RatingAverage = activeReviews.Count == 0
        ? 0
        : activeReviews.Average(r => r.Rating);

    await _context.SaveChangesAsync();
  }

  /// <summary>
  /// Mappar en review och tillhörande användare till ReviewDto.
  /// </summary>
  private static ReviewDto MapToReviewDto(Review review, ApplicationUser user)
  {
    return new ReviewDto
    {
      Id = review.Id,
      MovieId = review.MovieId,
      UserId = review.UserId,
      UserDisplayName = review.UseNickname && !string.IsNullOrWhiteSpace(user.Nickname)
            ? user.Nickname!
            : user.FullName,
      Comment = review.Comment ?? string.Empty,
      Rating = review.Rating,
      CreatedAt = review.CreatedAt,
      IsEdited = review.IsEdited
    };
  }
}
