using System.ComponentModel.DataAnnotations;
using RetroVHS.Shared.Enums;

namespace RetroVHS.Shared.DTOs.Movies;

/// <summary>
/// DTO som används när vi skapar eller uppdaterar en koppling
/// mellan en film och en person, t.ex. skådespelare eller regissör.
/// </summary>
public class CreateMovieCreditDto
{
    /// <summary>
    /// Id för personen som ska kopplas till filmen
    /// </summary>
    [Required]
    public int PersonId { get; set; }

    /// <summary>
    /// Personens roll i filmen, t.ex. Actor eller Director
    /// </summary>
    [Required]
    public CreditRole Role { get; set; }

    /// <summary>
    /// Namnet på karaktären som personen spelar, om rollen är Actor
    /// </summary>
    [StringLength(100)]
    public string? CharacterName { get; set; }

    /// <summary>
    /// Ordningen vi vill visa personen i credits-listan
    /// </summary>
    public int DisplayOrder { get; set; } = 0;
}