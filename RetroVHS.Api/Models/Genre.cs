using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Api.Models;

/// <summary>
/// Representerar en genre, t.ex. Action, Drama eller Komedi.
/// Vi använder en separat tabell för genre istället för text i Movie
/// för att undvika duplicering och göra filtrering enklare.
/// </summary>
public class Genre
{
    /// <summary>
    /// Primärnyckel för genren
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Namnet på genren (måste vara unikt)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    // =========================
    // Navigation properties
    // =========================

    /// <summary>
    /// Koppling till filmer via join tabellen MovieGenre
    /// Vi använder many to many eftersom en film kan ha flera genrer
    /// och en genre kan tillhöra flera filmer.
    /// </summary>
    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
}
