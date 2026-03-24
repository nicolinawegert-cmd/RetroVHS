using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.Reviews;

/// <summary>
/// DTO som används när en användare skapar en ny recension.
/// </summary>
public class CreateReviewDto
{
    /// <summary>
    /// Id på filmen som recenseras
    /// </summary>
    [Required]
    public int MovieId { get; set; }

    /// <summary>
    /// Själva recensionstexten
    /// </summary>
    [StringLength(1000)]
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Betyg mellan 1 och 5
    /// </summary>
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
}