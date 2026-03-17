using System.ComponentModel.DataAnnotations;
using RetroVHS.Shared.Enums;

namespace RetroVHS.Api.Models;

/// <summary>
/// Representerar en film i systemet.
/// Detta är en av de viktigaste entiteterna i hela applikationen.
/// En film kan ha genrer, skådespelare/regissörer (via MovieCredit),
/// recensioner (Review), samt koppling till uthyrning (Rental).
/// </summary>
public class Movie
{
    /// <summary>
    /// Primärnyckel för filmen
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Filmens titel (obligatorisk)
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Kort beskrivning av filmen
    /// Begränsad längd för att undvika för stora texter
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string Synopsis { get; set; } = string.Empty;

    /// <summary>
    /// Produktionsår (valideras mellan 1910 och nuvarande år)
    /// </summary>
    [Range(1910, 2100)]
    public int ReleaseYear { get; set; }

    /// <summary>
    /// Filmlängd i minuter (ex: 120 min)
    /// </summary>
    [Range(1, 600)]
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Pris för att hyra filmen
    /// </summary>
    [Range(typeof(decimal), "0.01", "999.99")]
    public decimal RentalPrice { get; set; }

    /// <summary>
    /// URL till filmens poster/bild
    /// </summary>
    [StringLength(500)]
    public string? PosterUrl { get; set; }

    /// <summary>
    /// URL till trailer (t.ex. YouTube)
    /// </summary>
    [StringLength(500)]
    public string? TrailerUrl { get; set; }

    /// <summary>
    /// Anger om filmen är tillgänglig, slutsåld, etc.
    /// </summary>
    public MovieAvailabilityStatus AvailabilityStatus { get; set; } = MovieAvailabilityStatus.Available;

    /// <summary>
    /// Antal tillgängliga "kopior" (valfritt. används om vi vill simulera lager)
    /// </summary>
    [Range(0, 10000)]
    public int StockQuantity { get; set; } = 0;

    /// <summary>
    /// Om filmen ska visas som "featured" (t.ex. på startsidan)
    /// </summary>
    public bool IsFeatured { get; set; } = false;

    /// <summary>
    /// Genomsnittligt betyg (räknas från reviews)
    /// </summary>
    [Range(0, 5)]
    public double RatingAverage { get; set; } = 0;

    /// <summary>
    /// Antal betyg (för att visa hur många som röstat)
    /// </summary>
    [Range(0, int.MaxValue)]
    public int RatingCount { get; set; } = 0;

    /// <summary>
    /// Koppling till produktionsbolag (valfri)
    /// </summary>
    public int? ProductionCompanyId { get; set; }
    public ProductionCompany? ProductionCompany { get; set; }

    // =========================
    // Navigation properties
    // =========================

    /// <summary>
    /// Koppling till genrer (many-to-many via MovieGenre)
    /// </summary>
    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();

    /// <summary>
    /// Koppling till personer (skådespelare/regissörer via MovieCredit)
    /// </summary>
    public ICollection<MovieCredit> MovieCredits { get; set; } = new List<MovieCredit>();

    /// <summary>
    /// Alla recensioner för filmen
    /// </summary>
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>
    /// Koppling till varukorg (om filmen ligger i någons cart)
    /// </summary>
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    /// <summary>
    /// Alla uthyrningar av filmen
    /// </summary>
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    /// <summary>
    /// Språk filmen är på
    /// </summary>
    [StringLength(50)]
    public string? Language { get; set; }

    /// <summary>
    /// Ursprungsland för filmen
    /// </summary>
    [StringLength(100)]
    public string? Country { get; set; }
}