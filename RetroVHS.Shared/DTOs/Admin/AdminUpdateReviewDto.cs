using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.Admin;

/// <summary>
/// DTO som används när admin redigerar en recensionskommentar.
/// </summary>
public class AdminUpdateReviewDto
{
    /// <summary>
    /// Uppdaterad kommentartext
    /// </summary>
    [StringLength(1000)]
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Uppdaterat betyg mellan 1 och 5
    /// </summary>
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
}
