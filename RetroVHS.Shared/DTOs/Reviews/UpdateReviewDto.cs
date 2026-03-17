using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.Reviews;

/// <summary>
/// DTO som används när en användare uppdaterar en befintlig recension.
/// </summary>
public class UpdateReviewDto
{
    /// <summary>
    /// Recensionens id
    /// </summary>
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// Uppdaterad recensionstext
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Uppdaterat betyg mellan 1 och 5
    /// </summary>
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
}