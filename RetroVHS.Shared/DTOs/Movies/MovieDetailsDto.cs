using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Shared.DTOs.Movies;

/// <summary>
/// DTO för att visa fullständig information om en film på detaljsidan.
/// Här samlar vi all data som frontend behöver för att kunna visa
/// synopsis, betyg, genrer, credits och recensioner.
/// </summary>
public class MovieDetailsDto
{
    /// <summary>
    /// Filmens id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Filmens titel
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Filmens beskrivning
    /// </summary>
    public string Synopsis { get; set; } = string.Empty;

    /// <summary>
    /// Filmens utgivningsår
    /// </summary>
    public int ReleaseYear { get; set; }

    /// <summary>
    /// Filmlängd i minuter
    /// </summary>
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Pris för att hyra filmen
    /// </summary>
    public decimal RentalPrice { get; set; }

    /// <summary>
    /// Filmens genomsnittliga betyg
    /// </summary>
    public double RatingAverage { get; set; }

    /// <summary>
    /// Antal betyg som filmen fått
    /// </summary>
    public int RatingCount { get; set; }

    /// <summary>
    /// URL till filmens poster
    /// </summary>
    public string? PosterUrl { get; set; }

    /// <summary>
    /// URL till trailer
    /// </summary>
    public string? TrailerUrl { get; set; }

    /// <summary>
    /// Filmens tillgänglighetsstatus som text
    /// </summary>
    public string AvailabilityStatus { get; set; } = string.Empty;

    /// <summary>
    /// Antal tillgängliga exemplar om vi vill simulera lager
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Anger om filmen ska lyftas extra
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Språk filmen är på
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Ursprungsland
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Id för produktionsbolag (om kopplat)
    /// </summary>
    public int? ProductionCompanyId { get; set; }

    /// <summary>
    /// Namnet på produktionsbolaget
    /// </summary>
    public string? ProductionCompanyName { get; set; }

    /// <summary>
    /// Lista med filmens genrer
    /// </summary>
    public List<string> Genres { get; set; } = new();

    /// <summary>
    /// Lista med regissörer kopplade till filmen
    /// </summary>
    public List<PersonCreditDto> Directors { get; set; } = new();

    /// <summary>
    /// Lista med skådespelare kopplade till filmen
    /// </summary>
    public List<PersonCreditDto> Cast { get; set; } = new();

    /// <summary>
    /// Lista med recensioner för filmen
    /// </summary>
    public List<ReviewDto> Reviews { get; set; } = new();
}