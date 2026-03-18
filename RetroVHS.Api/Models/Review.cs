using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Api.Models;

/// <summary>
/// Representerar en recension som en användare har skrivit på en film.
/// 
/// Vi samlar både betyg och kommentar i samma modell.
/// Det gör strukturen enklare och mer logisk eftersom en recension
/// hör ihop med både en användare och en specifik film.
/// </summary>
public class Review
{
    /// <summary>
    /// Primärnyckel för recensionen
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Betyg som användaren satt på filmen.
    /// Vi begränsar värdet till 1-5.
    /// </summary>
    [Range(1, 5)]
    public int Rating { get; set; }

    /// <summary>
    /// Valfri kommentar till betyget.
    /// </summary>
    [StringLength(1000)]
    public string? Comment { get; set; }

    /// <summary>
    /// Anger om användarens nickname ska visas istället för fullständigt namn.
    /// Detta gör att vi kan stödja frivillig anonymitet vid kommentarer.
    /// </summary>
    public bool UseNickname { get; set; } = true;

    /// <summary>
    /// Tidpunkt då recensionen skapades.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Tidpunkt då recensionen senast uppdaterades.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Visar om recensionen har redigerats efter att den skapades.
    /// </summary>
    public bool IsEdited { get; set; } = false;

    /// <summary>
    /// Mjuk borttagning av recension.
    /// Vi kan dölja recensionen utan att förlora historik i databasen.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Främmande nyckel till filmen som recensionen gäller
    /// </summary>
    public int MovieId { get; set; }

    /// <summary>
    /// Navigation till filmen
    /// </summary>
    public Movie Movie { get; set; } = null!;

    /// <summary>
    /// Främmande nyckel till användaren som skrev recensionen
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Navigation till användaren som skrev recensionen
    /// </summary>
    public ApplicationUser User { get; set; } = null!;
}