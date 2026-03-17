using System.ComponentModel.DataAnnotations;
using RetroVHS.Shared.Enums;

namespace RetroVHS.Api.Models;

/// <summary>
/// Representerar en uthyrning av en film.
/// 
/// När en användare hyr en film skapas en post i denna tabell.
/// Här sparar vi vem som hyrde filmen, vilken film det gäller,
/// vad användaren betalade och hur länge hyran är aktiv.
/// </summary>
public class Rental
{
    /// <summary>
    /// Primärnyckel för uthyrningen
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Främmande nyckel till användaren som hyrde filmen
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Navigation till användaren som hyrde filmen
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Främmande nyckel till filmen som hyrdes
    /// </summary>
    public int MovieId { get; set; }

    /// <summary>
    /// Navigation till filmen som hyrdes
    /// </summary>
    public Movie Movie { get; set; } = null!;

    /// <summary>
    /// Priset användaren betalade vid uthyrningstillfället.
    /// Vi sparar detta separat så att historiken inte påverkas
    /// om filmens pris ändras senare.
    /// </summary>
    [Range(typeof(decimal), "0.01", "999.99")]
    public decimal PricePaid { get; set; }

    /// <summary>
    /// Tidpunkt då filmen hyrdes
    /// </summary>
    public DateTime RentedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Tidpunkt då hyrperioden går ut
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Status för uthyrningen, t.ex. aktiv eller utgången
    /// </summary>
    public RentalStatus Status { get; set; } = RentalStatus.Active;

    /// <summary>
    /// Valfri referens till betalningen.
    /// Kan användas om vi vill simulera ett betalnings-id senare.
    /// </summary>
    [StringLength(100)]
    public string? PaymentReference { get; set; }

    /// <summary>
    /// Tidpunkt då användaren började titta på filmen, om vi vill följa detta senare
    /// </summary>
    public DateTime? WatchedAt { get; set; }
}