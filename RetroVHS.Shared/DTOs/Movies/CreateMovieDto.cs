using System.ComponentModel.DataAnnotations;
using RetroVHS.Shared.Enums;

namespace RetroVHS.Shared.DTOs.Movies;

/// <summary>
/// DTO som används när admin skapar en ny film.
/// Vi skickar bara den data som behövs för att bygga upp filmen
/// och dess relationer i backend.
/// </summary>
public class CreateMovieDto
{
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
    /// Id för produktionsbolaget om filmen har ett registrerat bolag
    /// </summary>
    public int? ProductionCompanyId { get; set; }

    /// <summary>
    /// Filmens tillgänglighetsstatus
    /// </summary>
    public MovieAvailabilityStatus AvailabilityStatus { get; set; } = MovieAvailabilityStatus.Available;

    /// <summary>
    /// Antal tillgängliga exemplar om vi vill simulera lager
    /// </summary>
    [Range(0, 10000)]
    public int StockQuantity { get; set; } = 0;

    /// <summary>
    /// Anger om filmen ska lyftas extra, t.ex. på startsidan
    /// </summary>
    public bool IsFeatured { get; set; } = false;

    /// <summary>
    /// Lista med genre-id:n som ska kopplas till filmen
    /// </summary>
    public List<int> GenreIds { get; set; } = new();

    /// <summary>
    /// Lista med credits som ska kopplas till filmen
    /// </summary>
    public List<CreateMovieCreditDto> Credits { get; set; } = new();
}