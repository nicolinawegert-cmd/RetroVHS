using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Api.Models;

/// <summary>
/// Representerar en användare i systemet.
/// 
/// Vi ärver från IdentityUser<int> för att få stöd för inloggning,
/// lösenordshantering, e-post, roller och annan säkerhetsfunktionalitet
/// som redan finns i ASP.NET Identity.
/// 
/// Här lägger vi till våra egna fält som behövs för filmplattformen,
/// till exempel namn, nickname och kopplingar till reviews, wishlist och rentals.
/// </summary>
public class ApplicationUser : IdentityUser<int>
{
    /// <summary>
    /// Förnamn på användaren
    /// </summary>
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Efternamn på användaren
    /// </summary>
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Valfritt nickname som kan visas t.ex. i recensioner och kommentarer
    /// </summary>
    [StringLength(50)]
    public string? Nickname { get; set; }

    /// <summary>
    /// Anger om användaren är blockerad.
    /// Detta kan vi använda för att stoppa uthyrning, recensioner
    /// eller inloggning beroende på hur vi vill bygga logiken senare.
    /// </summary>
    public bool IsBlocked { get; set; } = false;

    /// <summary>
    /// Tidpunkt då användaren skapades i systemet
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Tidpunkt då användarens profil senast uppdaterades
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // =========================
    // Hjälpfält
    // =========================

    /// <summary>
    /// Returnerar användarens fullständiga namn.
    /// Detta sparas inte i databasen utan används bara som hjälp i koden.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    // =========================
    // Navigation properties
    // =========================

    /// <summary>
    /// Alla recensioner som användaren har skrivit
    /// </summary>
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>
    /// Alla filmer som användaren har sparat i sin wishlist
    /// </summary>
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();

    /// <summary>
    /// Alla uthyrningar som användaren har gjort
    /// </summary>
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    /// <summary>
    /// Alla varukorgar som användaren haft.
    /// Vi bygger kanske Cart senare, men det är bra att ha med kopplingen redan nu.
    /// </summary>
    public ICollection<Cart> Carts { get; set; } = new List<Cart>();

    /// <summary>
    /// Alla refresh tokens som är kopplade till användaren.
    /// Dessa används för att kunna hämta nya access tokens utan att logga in igen.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}