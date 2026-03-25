namespace RetroVHS.Shared.DTOs.Wishlist;

/// <summary>
/// DTO som representerar en film i användarens wishlist.
/// </summary>
public class WishlistItemDto
{
    /// <summary>
    /// Filmens id
    /// </summary>
    public int MovieId { get; set; }

    /// <summary>
    /// Filmens titel
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Posterbild
    /// </summary>
    public string? PosterUrl { get; set; }

    /// <summary>
    /// Filmens pris
    /// </summary>
    public decimal RentalPrice { get; set; }

    /// <summary>
    /// När filmen lades till i wishlist
    /// </summary>
    public DateTime AddedAt { get; set; }
}