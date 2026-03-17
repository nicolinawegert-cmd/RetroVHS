using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Models;

namespace RetroVHS.Api.Data;

/// <summary>
/// Huvudkontext för databasen.
///
/// Vi låter kontexten ärva från IdentityDbContext eftersom vi använder
/// ASP.NET Identity tillsammans med JWT för inloggning och behörighet.
/// 
/// Det gör att vi får med tabeller för användare, roller, claims,
/// inloggningar och annan säkerhetsdata automatiskt.
/// 
/// Utöver Identity-tabellerna registrerar vi även våra egna tabeller
/// för filmer, recensioner, wishlist, hyror och varukorgar.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    /// <summary>
    /// Konstruktor som tar emot DbContextOptions från dependency injection.
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // =========================
    // DbSets för våra egna tabeller
    // =========================

    /// <summary>
    /// Filmer i systemet
    /// </summary>
    public DbSet<Movie> Movies { get; set; }

    /// <summary>
    /// Genrer som filmer kan tillhöra
    /// </summary>
    public DbSet<Genre> Genres { get; set; }

    /// <summary>
    /// Produktionsbolag kopplade till filmer
    /// </summary>
    public DbSet<ProductionCompany> ProductionCompanies { get; set; }

    /// <summary>
    /// Personer kopplade till filmer, t.ex. skådespelare och regissörer
    /// </summary>
    public DbSet<Person> Persons { get; set; }

    /// <summary>
    /// Join-tabell mellan Movie och Genre
    /// </summary>
    public DbSet<MovieGenre> MovieGenres { get; set; }

    /// <summary>
    /// Koppling mellan film, person och roll
    /// </summary>
    public DbSet<MovieCredit> MovieCredits { get; set; }

    /// <summary>
    /// Recensioner med betyg och kommentar
    /// </summary>
    public DbSet<Review> Reviews { get; set; }

    /// <summary>
    /// Sparade filmer i användarens wishlist
    /// </summary>
    public DbSet<WishlistItem> WishlistItems { get; set; }

    /// <summary>
    /// Uthyrningar av filmer
    /// </summary>
    public DbSet<Rental> Rentals { get; set; }

    /// <summary>
    /// Varukorgar
    /// </summary>
    public DbSet<Cart> Carts { get; set; }

    /// <summary>
    /// Filmer som ligger i en varukorg
    /// </summary>
    public DbSet<CartItem> CartItems { get; set; }

    /// <summary>
    /// Här konfigurerar vi relationer, nycklar, index och regler
    /// som inte räcker att beskriva enbart med data annotations.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // =========================
        // MovieGenre: kompositnyckel
        // =========================
        builder.Entity<MovieGenre>()
            .HasKey(mg => new { mg.MovieId, mg.GenreId });

        // =========================
        // Unika index
        // =========================

        // En genre ska inte kunna finnas flera gånger med samma namn
        builder.Entity<Genre>()
            .HasIndex(g => g.Name)
            .IsUnique();

        // Ett produktionsbolag ska inte kunna finnas flera gånger med samma namn
        builder.Entity<ProductionCompany>()
            .HasIndex(pc => pc.Name)
            .IsUnique();

        // En användare ska bara kunna recensera samma film en gång
        builder.Entity<Review>()
            .HasIndex(r => new { r.MovieId, r.UserId })
            .IsUnique();

        // En användare ska bara kunna lägga samma film i wishlist en gång
        builder.Entity<WishlistItem>()
            .HasIndex(w => new { w.UserId, w.MovieId })
            .IsUnique();

        // En varukorg ska inte kunna innehålla samma film flera gånger
        builder.Entity<CartItem>()
            .HasIndex(ci => new { ci.CartId, ci.MovieId })
            .IsUnique();

        // =========================
        // Relationer för Review
        // =========================
        builder.Entity<Review>()
            .HasOne(r => r.Movie)
            .WithMany(m => m.Reviews)
            .HasForeignKey(r => r.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Relationer för WishlistItem
        // =========================
        builder.Entity<WishlistItem>()
            .HasOne(w => w.User)
            .WithMany(u => u.WishlistItems)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<WishlistItem>()
            .HasOne(w => w.Movie)
            .WithMany()
            .HasForeignKey(w => w.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // Relationer för Rental
        // =========================
        builder.Entity<Rental>()
            .HasOne(r => r.User)
            .WithMany(u => u.Rentals)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Rental>()
            .HasOne(r => r.Movie)
            .WithMany(m => m.Rentals)
            .HasForeignKey(r => r.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Relationer för Cart
        // =========================
        builder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // Relationer för CartItem
        // =========================
        builder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CartItem>()
            .HasOne(ci => ci.Movie)
            .WithMany(m => m.CartItems)
            .HasForeignKey(ci => ci.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Relationer för Movie och ProductionCompany
        // =========================
        builder.Entity<Movie>()
            .HasOne(m => m.ProductionCompany)
            .WithMany(pc => pc.Movies)
            .HasForeignKey(m => m.ProductionCompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        // =========================
        // Relationer för MovieCredit
        // =========================
        builder.Entity<MovieCredit>()
            .HasOne(mc => mc.Movie)
            .WithMany(m => m.MovieCredits)
            .HasForeignKey(mc => mc.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MovieCredit>()
            .HasOne(mc => mc.Person)
            .WithMany(p => p.MovieCredits)
            .HasForeignKey(mc => mc.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // Relationer för MovieGenre
        // =========================
        builder.Entity<MovieGenre>()
            .HasOne(mg => mg.Movie)
            .WithMany(m => m.MovieGenres)
            .HasForeignKey(mg => mg.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MovieGenre>()
            .HasOne(mg => mg.Genre)
            .WithMany(g => g.MovieGenres)
            .HasForeignKey(mg => mg.GenreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}