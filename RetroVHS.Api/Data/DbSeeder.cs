using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Models;

namespace RetroVHS.Api.Data;

/// <summary>
/// Ansvarar för att lägga in grunddata i databasen vid appstart.
/// 
/// Här skapar vi:
/// - Roller (Admin, Member)
/// - Admin-användare
/// - Testanvändare
/// - Grunddata som filmer, genrer och personer (läggs till senare)
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

        // =========================
        // Säkerställ att databasen finns
        // =========================
       

        // =========================
        // Skapa roller
        // =========================
        var roles = new[] { "Admin", "Member" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }

        // =========================
        // Skapa admin
        // =========================
        var adminEmail = "admin@retrovhs.se";

        var adminUser = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (adminUser == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // =========================
        // Skapa test user
        // =========================
        var userEmail = "user@retrovhs.se";

        var testUser = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == userEmail);

        if (testUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail,
                FirstName = "Test",
                LastName = "User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "User123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Member");
            }
        }

        // =========================
        // Stoppa om data redan finns
        // =========================
        if (await context.Movies.AnyAsync())
            return;

        // =========================
        // Seed: Genres
        // =========================
        var genres = new List<Genre>
        {
            new Genre { Name = "Action" },
            new Genre { Name = "Drama" },
            new Genre { Name = "Comedy" },
            new Genre { Name = "Sci-Fi" },
            new Genre { Name = "Horror" },
            new Genre { Name = "Thriller" },
            new Genre { Name = "Crime" },
            new Genre { Name = "Romance" },
            new Genre { Name = "Adventure" }

        };

        context.Genres.AddRange(genres);
        await context.SaveChangesAsync();

        // =========================
        // Seed: ProductionCompany
        // =========================
        var company = new ProductionCompany
        {
            Name = "Retro Studios",
            Country = "USA"
        };

        context.ProductionCompanies.Add(company);
        await context.SaveChangesAsync();

        // =========================
        // Seed: Movies
        // =========================
        var movie1 = new Movie
        {
            Title = "Retro Action",
            Synopsis = "En klassisk actionfilm i retrostil.",
            ReleaseYear = 2005,
            DurationMinutes = 120,
            RentalPrice = 39,
            ProductionCompanyId = company.Id
        };

        context.Movies.Add(movie1);
        await context.SaveChangesAsync();

        // =========================
        // Koppla genre till film
        // =========================
        var actionGenre = genres.First(g => g.Name == "Action");

        context.MovieGenres.Add(new MovieGenre
        {
            MovieId = movie1.Id,
            GenreId = actionGenre.Id
        });

        await context.SaveChangesAsync();
    }
}