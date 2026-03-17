using RetroVHS.Shared.Enums;

namespace RetroVHS.Shared.DTOs.Movies;

/// <summary>
/// DTO som används för sökning, filtrering och sortering av filmer.
/// Vi använder denna för att samla alla filter i ett tydligt objekt
/// i stället för många separata query-parametrar.
/// </summary>
public class MovieFilterDto
{
    /// <summary>
    /// Söktext, t.ex. del av titel
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filtrera på genre
    /// </summary>
    public int? GenreId { get; set; }

    /// <summary>
    /// Filtrera på utgivningsår
    /// </summary>
    public int? ReleaseYear { get; set; }

    /// <summary>
    /// Lägsta pris
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Högsta pris
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Minsta genomsnittliga betyg
    /// </summary>
    public double? MinRating { get; set; }

    /// <summary>
    /// Filtrera på tillgänglighetsstatus
    /// </summary>
    public MovieAvailabilityStatus? AvailabilityStatus { get; set; }

    /// <summary>
    /// Fält att sortera på, t.ex. title, price, rating eller year
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Anger om sorteringen ska vara fallande
    /// </summary>
    public bool Desc { get; set; } = false;
}