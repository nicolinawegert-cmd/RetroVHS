using System.ComponentModel.DataAnnotations;
using RetroVHS.Shared.Enums;

namespace RetroVHS.Shared.DTOs.Movies;

/// <summary>
/// DTO som används när admin uppdaterar en befintlig film.
/// Liknar CreateMovieDto men innehåller även Id.
/// </summary>
public class UpdateMovieDto
{
    /// <summary>
    /// Filmens id
    /// </summary>
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// Filmens titel
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Filmens beskrivning
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string Synopsis { get; set; } = string.Empty;

    /// <summary>
    /// Filmens utgivningsår
    /// </summary>
    [Range(1910, 2100)]
    public int ReleaseYear { get; set; }

    /// <summary>
    /// Filmlängd i minuter
    /// </summary>
    [Range(1, 600)]
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Pris för att hyra filmen
    /// </summary>
    [Range(typeof(decimal), "0.01", "999.99")]
    public decimal RentalPrice { get; set; }

    /// <summary>
    /// URL till filmens poster
    /// </summary>
    [StringLength(500)]
    public string? PosterUrl { get; set; }

    /// <summary>
    /// URL till trailer
    /// </summary>
    [StringLength(500)]
    public string? TrailerUrl { get; set; }

    /// <summary>
    /// Språk filmen är på
    /// </summary>
    [StringLength(50)]
    public string? Language { get; set; }

    /// <summary>
    /// Ursprungsland
    /// </summary>
    [StringLength(100)]
    public string? Country { get; set; }

    /// <summary>
    /// Id för produktionsbolag
    /// </summary>
    public int? ProductionCompanyId { get; set; }

    /// <summary>
    /// Filmens tillgänglighetsstatus
    /// </summary>
    public MovieAvailabilityStatus AvailabilityStatus { get; set; }

    /// <summary>
    /// Antal tillgängliga exemplar
    /// </summary>
    [Range(0, 10000)]
    public int StockQuantity { get; set; }

    /// <summary>
    /// Anger om filmen ska lyftas extra
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Lista med genre-id:n
    /// </summary>
    public List<int> GenreIds { get; set; } = new();

    /// <summary>
    /// Lista med credits
    /// </summary>
    public List<CreateMovieCreditDto> Credits { get; set; } = new();
}