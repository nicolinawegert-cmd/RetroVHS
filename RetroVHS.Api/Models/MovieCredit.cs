using System.ComponentModel.DataAnnotations;
using RetroVHS.Shared.Enums;

namespace RetroVHS.Api.Models;

/// <summary>
/// Kopplar ihop en film med en person och anger vilken roll personen har i filmen.
/// 
/// Vi använder denna modell för att kunna återanvända samma Person-tabell
/// för t.ex. skådespelare, regissörer, producenter och manusförfattare.
/// </summary>
public class MovieCredit
{
    /// <summary>
    /// Primärnyckel för kopplingen
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Främmande nyckel till filmen
    /// </summary>
    public int MovieId { get; set; }

    /// <summary>
    /// Navigation till filmen
    /// </summary>
    public Movie Movie { get; set; } = null!;

    /// <summary>
    /// Främmande nyckel till personen
    /// </summary>
    public int PersonId { get; set; }

    /// <summary>
    /// Navigation till personen
    /// </summary>
    public Person Person { get; set; } = null!;

    /// <summary>
    /// Anger personens roll i filmen, t.ex. Actor eller Director.
    /// Vi använder enum för att undvika stavfel och få tydligare kod.
    /// </summary>
    public CreditRole Role { get; set; }

    /// <summary>
    /// Namnet på karaktären som personen spelar.
    /// Den här används främst när rollen är Actor.
    /// </summary>
    [StringLength(100)]
    public string? CharacterName { get; set; }

    /// <summary>
    /// Anger ordningen vi vill visa credits i på filmsidan.
    /// Exempel: huvudroll 1, nästa 2, osv.
    /// </summary>
    public int DisplayOrder { get; set; } = 0;
}