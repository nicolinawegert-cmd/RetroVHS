namespace RetroVHS.Api.Models;

/// <summary>
/// Representerar en film som en användare har sparat i sin wishlist.
///
/// Vi använder en egen tabell i stället för att lägga en lista med filmer direkt
/// på användaren, eftersom vi vill ha en tydlig och normaliserad koppling
/// mellan användare och film.
/// </summary>
public class WishlistItem
{
    /// <summary>
    /// Primärnyckel för wishlist-posten
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Främmande nyckel till användaren som sparat filmen
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Navigation till användaren som äger wishlist-posten
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Främmande nyckel till filmen som lagts i wishlist
    /// </summary>
    public int MovieId { get; set; }

    /// <summary>
    /// Navigation till filmen som lagts i wishlist
    /// </summary>
    public Movie Movie { get; set; } = null!;

    /// <summary>
    /// Tidpunkt då filmen lades till i wishlist
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}