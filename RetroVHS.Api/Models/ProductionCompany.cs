using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Api.Models;

/// <summary>
/// Representerar ett produktionsbolag som är kopplat till en eller flera filmer.
/// Vi använder en separat tabell för att undvika att samma bolagsnamn sparas
/// som fri text i flera filmer.
/// </summary>
public class ProductionCompany
{
    /// <summary>
    /// Primärnyckel för produktionsbolaget
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Namn på produktionsbolaget
    /// </summary>
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Land där bolaget är baserat
    /// Valfri uppgift som kan vara bra för visning och filtrering senare.
    /// </summary>
    [StringLength(100)]
    public string? Country { get; set; }

    // =========================
    // Navigation properties
    // =========================

    /// <summary>
    /// Alla filmer som är kopplade till detta produktionsbolag.
    /// Vi använder en one-to-many-relation här:
    /// ett produktionsbolag kan ha många filmer,
    /// men i vår första version har varje film ett produktionsbolag.
    /// </summary>
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}