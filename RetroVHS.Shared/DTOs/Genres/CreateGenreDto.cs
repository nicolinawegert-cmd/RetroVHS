using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.Genres;

public class CreateGenreDto
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}
