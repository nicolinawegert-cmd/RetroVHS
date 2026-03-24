using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Shared.DTOs.Wishlist;

namespace RetroVHS.Api.Services.Wishlists;

/// <summary>
/// Service som hanterar användarens önskelista.
/// </summary>
public class WishlistService : IWishlistService
{
  private readonly ApplicationDbContext _context;

  public WishlistService(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Hämtar alla filmer i användarens önskelista, sorterat på senast tillagd.
  /// </summary>
  public async Task<List<WishlistItemDto>> GetWishlistAsync(int userId)
  {
    return await _context.WishlistItems
        .Include(w => w.Movie)
        .Where(w => w.UserId == userId)
        .OrderByDescending(w => w.CreatedAt)
        .Select(w => new WishlistItemDto
        {
          MovieId = w.MovieId,
          Title = w.Movie.Title,
          PosterUrl = w.Movie.PosterUrl,
          AddedAt = w.CreatedAt
        })
        .ToListAsync();
  }

  /// <summary>
  /// Lägger till en film i önskelistan om den inte redan finns där.
  /// </summary>
  public async Task<(bool Success, string Message)> AddToWishlistAsync(int userId, int movieId)
  {
    var movie = await _context.Movies.FindAsync(movieId);
    if (movie == null)
      return (false, "Filmen hittades inte.");

    var exists = await _context.WishlistItems
        .AnyAsync(w => w.UserId == userId && w.MovieId == movieId);

    if (exists)
      return (false, "Filmen finns redan i din önskelista.");

    _context.WishlistItems.Add(new WishlistItem
    {
      UserId = userId,
      MovieId = movieId
    });

    await _context.SaveChangesAsync();
    return (true, "Filmen har lagts till i din önskelista.");
  }

  /// <summary>
  /// Tar bort en film från önskelistan.
  /// </summary>
  public async Task<(bool Success, string Message)> RemoveFromWishlistAsync(int userId, int movieId)
  {
    var item = await _context.WishlistItems
        .FirstOrDefaultAsync(w => w.UserId == userId && w.MovieId == movieId);

    if (item == null)
      return (false, "Filmen finns inte i din önskelista.");

    _context.WishlistItems.Remove(item);
    await _context.SaveChangesAsync();

    return (true, "Filmen har tagits bort från din önskelista.");
  }
}
