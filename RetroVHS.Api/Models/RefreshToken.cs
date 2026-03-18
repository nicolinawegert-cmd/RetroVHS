using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroVHS.Api.Models;

/// <summary>
/// Representerar en refresh token kopplad till en användare.
/// Används för att hämta nya access tokens utan att logga in igen.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Primärnyckel
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Själva token-strängen (ska vara unik)
    /// </summary>
    [Required]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// När token skapades
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// När token går ut
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// När token användes (för rotation)
    /// </summary>
    public DateTime? UsedAt { get; set; }

    /// <summary>
    /// När token blev ogiltig (logout eller rotation)
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// FK till användaren
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Navigation till användaren
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Anger om token är aktiv
    /// </summary>
    [NotMapped]
    public bool IsActive =>
        RevokedAt == null &&
        UsedAt == null &&
        ExpiresAt > DateTime.UtcNow;
}
