using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Api.Models;

/// <summary>
/// Representerar en person kopplad till en film,
/// t.ex. skådespelare, regissör, producent eller manusförfattare.
/// 
/// Vi använder en gemensam modell istället för separata klasser
/// som Actor och Director för att göra systemet mer flexibelt.
/// </summary>
public class Person
{
    /// <summary>
    /// Primärnyckel för personen
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Personens fullständiga namn
    /// </summary>
    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Födelsedatum (valfri)
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Land personen kommer ifrån (valfri)
    /// </summary>
    [StringLength(100)]
    public string? Country { get; set; }

    /// <summary>
    /// Kort biografi (valfri)
    /// </summary>
    [StringLength(1000)]
    public string? Bio { get; set; }

    // =========================
    // Navigation properties
    // =========================

    /// <summary>
    /// Koppling till filmer via MovieCredit.
    /// Här avgör vi också vilken roll personen har i filmen
    /// (t.ex. Actor eller Director).
    /// </summary>
    public ICollection<MovieCredit> MovieCredits { get; set; } = new List<MovieCredit>();
}