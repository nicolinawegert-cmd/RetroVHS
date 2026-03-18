using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Models;
using RetroVHS.Shared.Enums;

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
        var companies = new List<ProductionCompany>
        {
            new ProductionCompany { Name = "Hollywood Pictures", Country = "USA" },
            new ProductionCompany { Name = "Paramount Pictures", Country = "USA" },
            new ProductionCompany { Name = "Universal Pictures", Country = "USA" },
            new ProductionCompany { Name = "20th Century Fox", Country = "USA" },
            new ProductionCompany { Name = "Warner Bros.", Country = "USA" }
        };

        context.ProductionCompanies.AddRange(companies);
        await context.SaveChangesAsync();

        // =========================
        // Seed: Movies
        // =========================
        var movies = new List<Movie>
{
    new Movie
    {
        Title = "Avatar",
        Synopsis = "A paraplegic Marine dispatched to the moon Pandora on a unique mission becomes torn between following his orders and protecting the world he feels is his home.",
        ReleaseYear = 2009,
        DurationMinutes = 162,
        RentalPrice = 49,
        PosterUrl = "/posters/avatar.webp",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "20th Century Fox").Id
    },
    new Movie
    {
        Title = "Blood In Blood Out",
        Synopsis = "Based on the true life experiences of poet Jimmy Santiago Baca, the film follows the intertwining lives of three Chicano relatives from 1972 to 1984.",
        ReleaseYear = 1993,
        DurationMinutes = 180,
        RentalPrice = 39,
        PosterUrl = "/posters/blood-in-blood-out.webp",
        Language = "English",
        Country = "USA",
        StockQuantity = 3,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Hollywood Pictures").Id
    },
    new Movie
    {
        Title = "Pulp Fiction",
        Synopsis = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
        ReleaseYear = 1994,
        DurationMinutes = 154,
        RentalPrice = 45,
        PosterUrl = "/posters/pulp-fiction.jpg",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Paramount Pictures").Id
    },
    new Movie
    {
        Title = "Scarface",
        Synopsis = "In 1980 Miami, a determined Cuban immigrant takes over a drug cartel and succumbs to greed.",
        ReleaseYear = 1983,
        DurationMinutes = 170,
        RentalPrice = 39,
        PosterUrl = "/posters/scarface.webp",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Universal Pictures").Id
    },
    new Movie
    {
        Title = "The Godfather",
        Synopsis = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant youngest son.",
        ReleaseYear = 1972,
        DurationMinutes = 175,
        RentalPrice = 49,
        PosterUrl = "/posters/the-godfather.jpg",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Paramount Pictures").Id
    },
    new Movie
    {
        Title = "The Shawshank Redemption",
        Synopsis = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
        ReleaseYear = 1994,
        DurationMinutes = 142,
        RentalPrice = 45,
        PosterUrl = "/posters/the-shawshank-redemption.jpg",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "Titanic",
        Synopsis = "A seventeen-year-old aristocrat falls in love with a kind but poor artist aboard the luxurious, ill-fated R.M.S. Titanic.",
        ReleaseYear = 1997,
        DurationMinutes = 195,
        RentalPrice = 45,
        PosterUrl = "/posters/titanic.webp",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Paramount Pictures").Id
    }
};

        context.Movies.AddRange(movies);
        await context.SaveChangesAsync();

        // =========================
        // Seed: MovieGenre-kopplingar
        // =========================
        var avatar = movies.First(m => m.Title == "Avatar");
        var blood = movies.First(m => m.Title == "Blood In Blood Out");
        var pulp = movies.First(m => m.Title == "Pulp Fiction");
        var scarface = movies.First(m => m.Title == "Scarface");
        var godfather = movies.First(m => m.Title == "The Godfather");
        var shawshank = movies.First(m => m.Title == "The Shawshank Redemption");
        var titanic = movies.First(m => m.Title == "Titanic");

        var action = genres.First(g => g.Name == "Action");
        var drama = genres.First(g => g.Name == "Drama");
        var crime = genres.First(g => g.Name == "Crime");
        var scifi = genres.First(g => g.Name == "Sci-Fi");
        var romance = genres.First(g => g.Name == "Romance");
        var thriller = genres.First(g => g.Name == "Thriller");
        var adventure = genres.First(g => g.Name == "Adventure");

        context.MovieGenres.AddRange(
            new MovieGenre { MovieId = avatar.Id, GenreId = action.Id },
            new MovieGenre { MovieId = avatar.Id, GenreId = scifi.Id },
            new MovieGenre { MovieId = avatar.Id, GenreId = adventure.Id },
            new MovieGenre { MovieId = blood.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = blood.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = pulp.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = pulp.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = scarface.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = scarface.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = godfather.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = godfather.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = shawshank.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = titanic.Id, GenreId = romance.Id },
            new MovieGenre { MovieId = titanic.Id, GenreId = drama.Id }
        );

        await context.SaveChangesAsync();

        // =========================
        // Seed: Persons
        // =========================
        var persons = new List<Person>
{
    new Person { FullName = "James Cameron", Country = "Canada", Bio = "Director of Avatar and Titanic." },
    new Person { FullName = "Sam Worthington", Country = "Australia", Bio = "Known for Avatar." },
    new Person { FullName = "Zoe Saldana", Country = "USA", Bio = "Known for Avatar and Guardians of the Galaxy." },
    new Person { FullName = "Taylor Hackford", Country = "USA", Bio = "Director of Blood In Blood Out." },
    new Person { FullName = "Damian Chapa", Country = "USA", Bio = "Known for Blood In Blood Out." },
    new Person { FullName = "Jesse Borrego", Country = "USA", Bio = "Known for Blood In Blood Out." },
    new Person { FullName = "Quentin Tarantino", Country = "USA", Bio = "Director of Pulp Fiction and Kill Bill." },
    new Person { FullName = "John Travolta", Country = "USA", Bio = "Known for Pulp Fiction and Grease." },
    new Person { FullName = "Samuel L. Jackson", Country = "USA", Bio = "Known for Pulp Fiction and The Avengers." },
    new Person { FullName = "Brian De Palma", Country = "USA", Bio = "Director of Scarface." },
    new Person { FullName = "Al Pacino", Country = "USA", Bio = "Known for Scarface and The Godfather." },
    new Person { FullName = "Michelle Pfeiffer", Country = "USA", Bio = "Known for Scarface and Batman Returns." },
    new Person { FullName = "Francis Ford Coppola", Country = "USA", Bio = "Director of The Godfather." },
    new Person { FullName = "Marlon Brando", Country = "USA", Bio = "Known for The Godfather." },
    new Person { FullName = "Frank Darabont", Country = "USA", Bio = "Director of The Shawshank Redemption." },
    new Person { FullName = "Tim Robbins", Country = "USA", Bio = "Known for The Shawshank Redemption." },
    new Person { FullName = "Morgan Freeman", Country = "USA", Bio = "Known for The Shawshank Redemption." },
    new Person { FullName = "Leonardo DiCaprio", Country = "USA", Bio = "Known for Titanic and The Wolf of Wall Street." },
    new Person { FullName = "Kate Winslet", Country = "UK", Bio = "Known for Titanic." }
};

        context.Persons.AddRange(persons);
        await context.SaveChangesAsync();

        // =========================
        // Seed: MovieCredits
        // =========================
        context.MovieCredits.AddRange(
            // Avatar
            new MovieCredit { MovieId = avatar.Id, PersonId = persons.First(p => p.FullName == "James Cameron").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = avatar.Id, PersonId = persons.First(p => p.FullName == "Sam Worthington").Id, Role = CreditRole.Actor, CharacterName = "Jake Sully", DisplayOrder = 2 },
            new MovieCredit { MovieId = avatar.Id, PersonId = persons.First(p => p.FullName == "Zoe Saldana").Id, Role = CreditRole.Actor, CharacterName = "Neytiri", DisplayOrder = 3 },

            // Blood In Blood Out
            new MovieCredit { MovieId = blood.Id, PersonId = persons.First(p => p.FullName == "Taylor Hackford").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = blood.Id, PersonId = persons.First(p => p.FullName == "Damian Chapa").Id, Role = CreditRole.Actor, CharacterName = "Miklo", DisplayOrder = 2 },
            new MovieCredit { MovieId = blood.Id, PersonId = persons.First(p => p.FullName == "Jesse Borrego").Id, Role = CreditRole.Actor, CharacterName = "Cruz", DisplayOrder = 3 },

            // Pulp Fiction
            new MovieCredit { MovieId = pulp.Id, PersonId = persons.First(p => p.FullName == "Quentin Tarantino").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = pulp.Id, PersonId = persons.First(p => p.FullName == "John Travolta").Id, Role = CreditRole.Actor, CharacterName = "Vincent Vega", DisplayOrder = 2 },
            new MovieCredit { MovieId = pulp.Id, PersonId = persons.First(p => p.FullName == "Samuel L. Jackson").Id, Role = CreditRole.Actor, CharacterName = "Jules Winnfield", DisplayOrder = 3 },

            // Scarface
            new MovieCredit { MovieId = scarface.Id, PersonId = persons.First(p => p.FullName == "Brian De Palma").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = scarface.Id, PersonId = persons.First(p => p.FullName == "Al Pacino").Id, Role = CreditRole.Actor, CharacterName = "Tony Montana", DisplayOrder = 2 },
            new MovieCredit { MovieId = scarface.Id, PersonId = persons.First(p => p.FullName == "Michelle Pfeiffer").Id, Role = CreditRole.Actor, CharacterName = "Elvira Hancock", DisplayOrder = 3 },

            // The Godfather
            new MovieCredit { MovieId = godfather.Id, PersonId = persons.First(p => p.FullName == "Francis Ford Coppola").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = godfather.Id, PersonId = persons.First(p => p.FullName == "Al Pacino").Id, Role = CreditRole.Actor, CharacterName = "Michael Corleone", DisplayOrder = 2 },
            new MovieCredit { MovieId = godfather.Id, PersonId = persons.First(p => p.FullName == "Marlon Brando").Id, Role = CreditRole.Actor, CharacterName = "Vito Corleone", DisplayOrder = 3 },

            // The Shawshank Redemption
            new MovieCredit { MovieId = shawshank.Id, PersonId = persons.First(p => p.FullName == "Frank Darabont").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = shawshank.Id, PersonId = persons.First(p => p.FullName == "Tim Robbins").Id, Role = CreditRole.Actor, CharacterName = "Andy Dufresne", DisplayOrder = 2 },
            new MovieCredit { MovieId = shawshank.Id, PersonId = persons.First(p => p.FullName == "Morgan Freeman").Id, Role = CreditRole.Actor, CharacterName = "Red", DisplayOrder = 3 },

            // Titanic
            new MovieCredit { MovieId = titanic.Id, PersonId = persons.First(p => p.FullName == "James Cameron").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = titanic.Id, PersonId = persons.First(p => p.FullName == "Leonardo DiCaprio").Id, Role = CreditRole.Actor, CharacterName = "Jack Dawson", DisplayOrder = 2 },
            new MovieCredit { MovieId = titanic.Id, PersonId = persons.First(p => p.FullName == "Kate Winslet").Id, Role = CreditRole.Actor, CharacterName = "Rose DeWitt Bukater", DisplayOrder = 3 }
        );

        await context.SaveChangesAsync();
    }
}