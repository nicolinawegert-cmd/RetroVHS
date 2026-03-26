using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Api.Services.Reviews;
using RetroVHS.Shared.DTOs.Admin;
using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Rentals;
using RetroVHS.Shared.DTOs.Reviews;
using RetroVHS.Shared.Enums;

namespace RetroVHS.Api.Services.Admin;

/// <summary>
/// Service som hanterar all administrativ affärslogik.
/// Samlar användarhantering, recensionsmoderering, beställningshantering
/// och dashboard-statistik på ett ställe.
/// </summary>
public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;
    private readonly IReviewService _reviewService;

    public AdminService(ApplicationDbContext context, IReviewService reviewService)
    {
        _context = context;
        _reviewService = reviewService;
    }

    // ========== Dashboard ==========

    public async Task<AdminDashboardDto> GetDashboardStatsAsync()
    {
        return new AdminDashboardDto
        {
            TotalUsers = await _context.Users.CountAsync(),
            BlockedUsers = await _context.Users.CountAsync(u => u.IsBlocked),
            TotalMovies = await _context.Movies.CountAsync(),
            ActiveRentals = await _context.Rentals.CountAsync(r => r.Status == RentalStatus.Active),
            TotalRentals = await _context.Rentals.CountAsync(r => r.Status == RentalStatus.Completed),
            CancelledRentals = await _context.Rentals.CountAsync(r => r.Status == RentalStatus.Cancelled),
            TotalReviews = await _context.Reviews.CountAsync(r => !r.IsDeleted)
        };
    }

    // ========== Användare ==========

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync();

        return users.Select(MapToUserDto).ToList();
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<(bool Success, string Message)> UpdateNicknameAsync(int userId, AdminSetNicknameDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return (false, "Användaren hittades inte.");

        user.Nickname = dto.Nickname;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, "Nickname har uppdaterats.");
    }

    public async Task<(bool Success, string Message)> DeleteUserAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Reviews)
            .Include(u => u.Rentals)
            .Include(u => u.WishlistItems)
            .Include(u => u.Carts).ThenInclude(c => c.Items)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return (false, "Användaren hittades inte.");

        // Ta bort relaterad data
        _context.Reviews.RemoveRange(user.Reviews);
        _context.Rentals.RemoveRange(user.Rentals);
        _context.WishlistItems.RemoveRange(user.WishlistItems);
        foreach (var cart in user.Carts)
            _context.CartItems.RemoveRange(cart.Items);
        _context.Carts.RemoveRange(user.Carts);
        _context.RefreshTokens.RemoveRange(user.RefreshTokens);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        // Uppdatera betyg för filmer som påverkades
        var affectedMovieIds = user.Reviews.Select(r => r.MovieId).Distinct().ToList();
        foreach (var movieId in affectedMovieIds)
            await _reviewService.UpdateMovieRatingAsync(movieId);

        return (true, "Användaren och all relaterad data har raderats.");
    }

    public async Task<(bool Success, string Message)> BlockUserAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return (false, "Användaren hittades inte.");
        if (user.IsBlocked) return (false, "Användaren är redan blockerad.");

        user.IsBlocked = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return (true, "Användaren har blockerats.");
    }

    public async Task<(bool Success, string Message)> UnblockUserAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return (false, "Användaren hittades inte.");
        if (!user.IsBlocked) return (false, "Användaren är inte blockerad.");

        user.IsBlocked = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return (true, "Användaren har avblockerats.");
    }

    // ========== Recensioner ==========

    public async Task<List<ReviewDto>> GetUserReviewsAsync(int userId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Movie)
            .Where(r => r.UserId == userId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                MovieId = r.MovieId,
                UserId = r.UserId,
                UserDisplayName = r.UseNickname && !string.IsNullOrWhiteSpace(r.User.Nickname)
                    ? r.User.Nickname!
                    : r.User.FullName,
                Comment = r.Comment ?? string.Empty,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt,
                IsEdited = r.IsEdited,
                MovieTitle = r.Movie.Title
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string Message)> RemoveReviewCommentAsync(int reviewId)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId && !r.IsDeleted);
        if (review == null) return (false, "Recensionen hittades inte.");

        review.Comment = null;
        review.IsEdited = true;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (true, "Kommentartexten har tagits bort, betyget är kvar.");
    }

    public async Task<(bool Success, string Message)> DeleteReviewAsync(int reviewId)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId && !r.IsDeleted);
        if (review == null) return (false, "Recensionen hittades inte.");

        review.IsDeleted = true;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await _reviewService.UpdateMovieRatingAsync(review.MovieId);

        return (true, "Recensionen har raderats.");
    }

    // ========== Beställningar ==========

    public async Task<List<RentalDto>> GetUserRentalsAsync(int userId)
    {
        var rentals = await _context.Rentals
            .Include(r => r.Movie)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.RentedAt)
            .ToListAsync();

        return rentals.Select(MapToRentalDto).ToList();
    }

    public async Task<(bool Success, string Message)> CancelRentalAsync(int rentalId)
    {
        var rental = await _context.Rentals
            .Include(r => r.Movie)
            .FirstOrDefaultAsync(r => r.Id == rentalId);

        if (rental == null)
            return (false, "Beställningen hittades inte.");

        if (rental.Status == RentalStatus.Cancelled)
            return (false, "Beställningen är redan avbruten.");

        if (rental.Status != RentalStatus.Active)
            return (false, "Bara aktiva beställningar kan avbrytas.");

        rental.Status = RentalStatus.Cancelled;

        // Återställ lagersaldo
        rental.Movie.StockQuantity++;
        if (rental.Movie.AvailabilityStatus == MovieAvailabilityStatus.OutOfStock)
            rental.Movie.AvailabilityStatus = MovieAvailabilityStatus.Available;

        await _context.SaveChangesAsync();

        return (true, "Beställningen har avbrutits och lagersaldot har återställts.");
    }

    // ========== Hjälpmetoder ==========

    private static UserDto MapToUserDto(ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Nickname = user.Nickname,
            Email = user.Email ?? string.Empty,
            IsBlocked = user.IsBlocked
        };
    }

    private static RentalDto MapToRentalDto(Rental rental)
    {
        return new RentalDto
        {
            Id = rental.Id,
            MovieId = rental.MovieId,
            Title = rental.Movie.Title,
            PricePaid = rental.PricePaid,
            RentedAt = rental.RentedAt,
            ExpiresAt = rental.ExpiresAt,
            Status = rental.Status
        };
    }

    }
