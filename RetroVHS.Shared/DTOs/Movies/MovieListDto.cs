namespace RetroVHS.Shared.DTOs.Movies;

/// <summary>
/// DTO för att visa en film i listor, kort och topplistor.
/// Vi skickar bara den information som behövs i översiktsvyer,
/// inte all detaljerad data.
/// </summary>
public class MovieListDto
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
    /// Filmens tillgänglighetsstatus som text
    /// </summary>
    public string AvailabilityStatus { get; set; } = string.Empty;

    /// <summary>
    /// Anger om filmen ska lyftas extra, t.ex. på startsidan
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Lista med genrenamn för visning i UI
    /// </summary>
    public List<string> Genres { get; set; } = new();
}