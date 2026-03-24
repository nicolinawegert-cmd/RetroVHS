using RetroVHS.Shared.DTOs.Wishlist;

namespace RetroVHS.Api.Services.Wishlists;

/// <summary>
/// Interface för hantering av användarens önskelista.
/// </summary>
public interface IWishlistService
{
  /// <summary>
  /// Hämtar alla filmer i användarens önskelista.
  /// </summary>
  Task<List<WishlistItemDto>> GetWishlistAsync(int userId);

  /// <summary>
  /// Lägger till en film i önskelistan.
  /// </summary>
  Task<(bool Success, string Message)> AddToWishlistAsync(int userId, int movieId);

  /// <summary>
  /// Tar bort en film från önskelistan.
  /// </summary>
  Task<(bool Success, string Message)> RemoveFromWishlistAsync(int userId, int movieId);
}
