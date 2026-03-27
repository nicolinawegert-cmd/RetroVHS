using RetroVHS.Shared.DTOs.Wishlist;

namespace RetroVHS.Client.Services;

/// <summary>
/// Kontrakt för klientens kommunikation med önskeliste-API:t.
/// </summary>
public interface IWishlistClient
{
    /// <summary>
    /// Hämtar användarens önskelista.
    /// </summary>
    Task<List<WishlistItemDto>> GetWishlistAsync();

    /// <summary>
    /// Lägger till en film i önskelistan.
    /// </summary>
    Task<bool> AddToWishlistAsync(int movieId);

    /// <summary>
    /// Tar bort en film från önskelistan.
    /// </summary>
    Task<bool> RemoveFromWishlistAsync(int movieId);
}
