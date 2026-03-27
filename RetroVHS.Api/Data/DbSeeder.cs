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
            new Genre { Name = "Adventure" },
            new Genre { Name = "War" },
            new Genre { Name = "Western" },
            new Genre { Name = "Fantasy" }
        };

        var existingGenreNames = await context.Genres
            .Select(g => g.Name)
            .ToListAsync();

        var missingGenres = genres
            .Where(g => !existingGenreNames.Contains(g.Name))
            .ToList();

        if (missingGenres.Count > 0)
        {
            context.Genres.AddRange(missingGenres);
            await context.SaveChangesAsync();
        }

        genres = await context.Genres.ToListAsync();

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

        var existingCompanyNames = await context.ProductionCompanies
            .Select(c => c.Name)
            .ToListAsync();

        var missingCompanies = companies
            .Where(c => !existingCompanyNames.Contains(c.Name))
            .ToList();

        if (missingCompanies.Count > 0)
        {
            context.ProductionCompanies.AddRange(missingCompanies);
            await context.SaveChangesAsync();
        }

        companies = await context.ProductionCompanies.ToListAsync();

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
        TrailerUrl = "https://www.youtube.com/watch?v=5PSNL1qE6VY",
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
        TrailerUrl = "https://www.youtube.com/watch?v=NMj89zgI8Yc",
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
        TrailerUrl = "https://www.youtube.com/watch?v=s7EdQ4FqbhY",
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
        TrailerUrl = "https://www.youtube.com/watch?v=7pQQHnqBa2E",
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
        TrailerUrl = "https://www.youtube.com/watch?v=UaVTIH8mujA",
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
        TrailerUrl = "https://www.youtube.com/watch?v=PLl99DlL6b4",
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
        TrailerUrl = "https://www.youtube.com/watch?v=kVrqfYjkTdQ",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Paramount Pictures").Id
    },
    new Movie
    {
        Title = "The Matrix",
        Synopsis = "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.",
        ReleaseYear = 1999,
        DurationMinutes = 136,
        RentalPrice = 45,
        PosterUrl = "/posters/the-matrix.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=vKQi3bBA1y8",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "Goodfellas",
        Synopsis = "The story of Henry Hill and his life in the mob, covering his relationship with his wife Karen Hill and his mob partners.",
        ReleaseYear = 1990,
        DurationMinutes = 146,
        RentalPrice = 42,
        PosterUrl = "/posters/goodfellas.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=qo5jJpHtI1Y",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "Terminator 2: Judgment Day",
        Synopsis = "A cyborg, identical to the one who failed to kill Sarah Connor, must now protect her teenage son from a more advanced and powerful cyborg.",
        ReleaseYear = 1991,
        DurationMinutes = 137,
        RentalPrice = 45,
        PosterUrl = "/posters/terminator-2.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=CRRlbK5w8AE",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "20th Century Fox").Id
    },
    new Movie
    {
        Title = "Fight Club",
        Synopsis = "An insomniac office worker and a devil-may-care soap maker form an underground fight club that evolves into much more.",
        ReleaseYear = 1999,
        DurationMinutes = 139,
        RentalPrice = 42,
        PosterUrl = "/posters/fight-club.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=qtRKdVHc-cE",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "20th Century Fox").Id
    },
    new Movie
    {
        Title = "Forrest Gump",
        Synopsis = "The presidencies of Kennedy and Johnson, the Vietnam War, and other historical events unfold from the perspective of an Alabama man with an IQ of 75.",
        ReleaseYear = 1994,
        DurationMinutes = 142,
        RentalPrice = 42,
        PosterUrl = "/posters/forrest-gump.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=bLvqoHBptjg",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Paramount Pictures").Id
    },
    new Movie
    {
        Title = "Die Hard",
        Synopsis = "A New York City police officer tries to save his estranged wife and several others taken hostage by terrorists during a Christmas party.",
        ReleaseYear = 1988,
        DurationMinutes = 132,
        RentalPrice = 39,
        PosterUrl = "/posters/die-hard.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=jaJuwKCmJbY",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "20th Century Fox").Id
    },
    new Movie
    {
        Title = "Alien",
        Synopsis = "The crew of a commercial spacecraft encounter a deadly lifeform after investigating an unknown transmission.",
        ReleaseYear = 1979,
        DurationMinutes = 117,
        RentalPrice = 39,
        PosterUrl = "/posters/alien.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=LjLamj-b0I8",
        Language = "English",
        Country = "USA",
        StockQuantity = 3,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "20th Century Fox").Id
    },
    new Movie
    {
        Title = "Predator",
        Synopsis = "A team of commandos on a mission in a Central American jungle encounter an extraterrestrial warrior that hunts them for sport.",
        ReleaseYear = 1987,
        DurationMinutes = 107,
        RentalPrice = 39,
        PosterUrl = "/posters/predator.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=XY6d4LGd8YM",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "20th Century Fox").Id

    },
        new Movie
    {
        Title = "A Nightmare on Elm Street",
        Synopsis = "The monstrous spirit of a slain child murderer seeks revenge by invading the dreams of teenagers whose parents were responsible for his untimely death.",
        ReleaseYear = 1984,
        DurationMinutes = 91,
        RentalPrice = 39,
        PosterUrl = "/posters/a-nightmare-on-elm-street.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=dCVh4lBfW-c",
        Language = "English",
        Country = "USA",
        StockQuantity = 3,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
        new Movie
    {
        Title = "Friday the 13th",
        Synopsis = "A group of camp counselors trying to reopen a summer camp are stalked and murdered by an unknown assailant.",
        ReleaseYear = 1980,
        DurationMinutes = 95,
        RentalPrice = 35,
        PosterUrl = "/posters/friday-the-13th.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=sAzkW7HFh5U",
        Language = "English",
        Country = "USA",
        StockQuantity = 3,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Paramount Pictures").Id
    },
    new Movie
    {
        Title = "Gladiator",
        Synopsis = "A former Roman General sets out to exact vengeance against the corrupt emperor who murdered his family and sent him into slavery.",
        ReleaseYear = 2000,
        DurationMinutes = 155,
        RentalPrice = 45,
        PosterUrl = "/posters/gladiator.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=owK1qxDselE",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Universal Pictures").Id
    },
    new Movie
    {
        Title = "Halloween",
        Synopsis = "Fifteen years after murdering his sister on Halloween night, Michael Myers escapes from a mental hospital and returns to the small town of Haddonfield to kill again.",
        ReleaseYear = 1978,
        DurationMinutes = 91,
        RentalPrice = 35,
        PosterUrl = "/posters/halloween.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=T5ke9IPTIJQ",
        Language = "English",
        Country = "USA",
        StockQuantity = 3,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Universal Pictures").Id
    },
    new Movie
    {
        Title = "Hellraiser",
        Synopsis = "A woman discovers the newly resurrected, partially formed body of her brother-in-law, who was killed by demons called Cenobites.",
        ReleaseYear = 1987,
        DurationMinutes = 94,
        RentalPrice = 35,
        PosterUrl = "/posters/hellraiser.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=8mOn4h0lgKQ",
        Language = "English",
        Country = "UK",
        StockQuantity = 3,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "Inception",
        Synopsis = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
        ReleaseYear = 2010,
        DurationMinutes = 148,
        RentalPrice = 49,
        PosterUrl = "/posters/inception.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=YoHD9XEInc0",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "Jurassic Park",
        Synopsis = "A pragmatic paleontologist visiting an almost complete theme park on an island is tasked with protecting a couple of kids after a power failure causes the park's cloned dinosaurs to run loose.",
        ReleaseYear = 1993,
        DurationMinutes = 127,
        RentalPrice = 45,
        PosterUrl = "/posters/jurassic-park.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=lc0UehYemQA",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Universal Pictures").Id
    },
    new Movie
    {
        Title = "Rocky",
        Synopsis = "A small-time Philadelphia boxer gets a supremely rare chance to fight the world heavyweight champion in a bout in which he strives to go the distance for his self-respect.",
        ReleaseYear = 1976,
        DurationMinutes = 120,
        RentalPrice = 39,
        PosterUrl = "/posters/rocky.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=-Hk-LYcavrw",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Universal Pictures").Id
    },
    new Movie
    {
        Title = "Saving Private Ryan",
        Synopsis = "Following the Normandy Landings, a group of U.S. soldiers go behind enemy lines to retrieve a paratrooper whose brothers have been killed in action.",
        ReleaseYear = 1998,
        DurationMinutes = 169,
        RentalPrice = 45,
        PosterUrl = "/posters/saving-private-ryan.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=zwhP5b4tD6g",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Paramount Pictures").Id
    },
    new Movie
    {
        Title = "The Dark Knight",
        Synopsis = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
        ReleaseYear = 2008,
        DurationMinutes = 152,
        RentalPrice = 49,
        PosterUrl = "/posters/the-dark-knight.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=EXeTwQWrcwY",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "The Exorcist",
        Synopsis = "When a teenage girl is possessed by a mysterious entity, her mother seeks the help of two priests to save her daughter.",
        ReleaseYear = 1973,
        DurationMinutes = 122,
        RentalPrice = 39,
        PosterUrl = "/posters/the-exorcist.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=YDGw1MTEe9k",
        Language = "English",
        Country = "USA",
        StockQuantity = 3,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "The Shining",
        Synopsis = "A family heads to an isolated hotel for the winter where a sinister presence influences the father into violence, while his psychic son sees horrific forebodings from both past and future.",
        ReleaseYear = 1980,
        DurationMinutes = 146,
        RentalPrice = 42,
        PosterUrl = "/posters/the-shining.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=5Cb3ik6zP2I",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "The Texas Chain Saw Massacre",
        Synopsis = "A group of friends visiting their grandfather's house in the country are hunted and terrorized by a chainsaw-wielding killer and his family of grave-robbing cannibals.",
        ReleaseYear = 1974,
        DurationMinutes = 83,
        RentalPrice = 35,
        PosterUrl = "/posters/the-texas-chainsaw-massacre.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=SyqfjW5iRcM",
        Language = "English",
        Country = "USA",
        StockQuantity = 3,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "Gran Torino",
        Synopsis = "Disgruntled Korean War veteran Walt Kowalski sets out to reform his neighbor, a Hmong teenager who tried to steal his prized possession: a 1972 Gran Torino.",
        ReleaseYear = 2008,
        DurationMinutes = 116,
        RentalPrice = 42,
        PosterUrl = "/posters/gran-torino.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=RMhbr2XQblk",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "Cast Away",
        Synopsis = "A FedEx executive undergoes a physical and emotional transformation after crash landing on a deserted island.",
        ReleaseYear = 2000,
        DurationMinutes = 143,
        RentalPrice = 42,
        PosterUrl = "/posters/cast-away.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=qGuOZPwLayY",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "20th Century Fox").Id
    },
    new Movie
    {
        Title = "Jaws",
        Synopsis = "When a killer shark unleashes chaos on a beach community off Long Island, it's up to a local sheriff, a marine biologist, and an old seafarer to hunt the beast down.",
        ReleaseYear = 1975,
        DurationMinutes = 124,
        RentalPrice = 42,
        PosterUrl = "/posters/jaws.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=U1fu_sA7XhE",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Universal Pictures").Id
    },
    new Movie
    {
        Title = "Se7en",
        Synopsis = "Two detectives, a rookie and a veteran, hunt a serial killer who uses the seven deadly sins as his motives.",
        ReleaseYear = 1995,
        DurationMinutes = 127,
        RentalPrice = 42,
        PosterUrl = "/posters/se7en.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=znmZoVkCjpI",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "American History X",
        Synopsis = "A former neo-nazi skinhead tries to prevent his younger brother from going down the same wrong path that he did.",
        ReleaseYear = 1998,
        DurationMinutes = 119,
        RentalPrice = 42,
        PosterUrl = "/posters/american-history-x.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=XfQYHqsiN5g",
        Language = "English",
        Country = "USA",
        StockQuantity = 3,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "Taxi Driver",
        Synopsis = "A mentally unstable veteran works as a nighttime taxi driver in New York City, where the perceived decadence and sleaze fuels his urge for violent action.",
        ReleaseYear = 1976,
        DurationMinutes = 114,
        RentalPrice = 39,
        PosterUrl = "/posters/taxi-driver.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=UUxD4-dEzn0",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "Casino",
        Synopsis = "A tale of greed, deception, money, power, and murder occur between two best friends: a mafia enforcer and a casino executive compete against each other over a gambling empire.",
        ReleaseYear = 1995,
        DurationMinutes = 178,
        RentalPrice = 42,
        PosterUrl = "/posters/casino.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=EJXDMwGWhoA",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Universal Pictures").Id
    },
    new Movie
    {
        Title = "No Country for Old Men",
        Synopsis = "Violence and mayhem ensue after a hunter stumbles upon a drug deal gone wrong and more than two million dollars in cash near the Rio Grande.",
        ReleaseYear = 2007,
        DurationMinutes = 122,
        RentalPrice = 42,
        PosterUrl = "/posters/no-country-for-old-men.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=38A__WT3-o0",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Paramount Pictures").Id
    },
    new Movie
    {
        Title = "The Mask",
        Synopsis = "Bank clerk Stanley Ipkiss is transformed into a manic superhero when he wears a mysterious mask.",
        ReleaseYear = 1994,
        DurationMinutes = 101,
        RentalPrice = 35,
        PosterUrl = "/posters/the-mask.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=LZl69yk5lEY",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "The Thing",
        Synopsis = "A research team in Antarctica is hunted by a shape-shifting alien that assumes the appearance of its victims.",
        ReleaseYear = 1982,
        DurationMinutes = 109,
        RentalPrice = 39,
        PosterUrl = "/posters/the-thing.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=5ftmr17M-a4",
        Language = "English",
        Country = "USA",
        StockQuantity = 3,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Universal Pictures").Id
    },
    new Movie
    {
        Title = "Bloodsport",
        Synopsis = "Frank Dux travels to Hong Kong to enter an illegal underground martial arts tournament called the Kumite.",
        ReleaseYear = 1988,
        DurationMinutes = 92,
        RentalPrice = 35,
        PosterUrl = "/posters/bloodsport.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=bCZ1CIQ64YQ",
        Language = "English",
        Country = "USA",
        StockQuantity = 3,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "Big Trouble in Little China",
        Synopsis = "A tough-talking trucker and his friend get caught up in a centuries-old mystical battle in San Francisco's Chinatown.",
        ReleaseYear = 1986,
        DurationMinutes = 99,
        RentalPrice = 35,
        PosterUrl = "/posters/big-trouble-in-little-china.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=592EiTD2Hgo",
        Language = "English",
        Country = "USA",
        StockQuantity = 3,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "20th Century Fox").Id
    },
         new Movie
    {
        Title = "Heat",
        Synopsis = "A group of high-end professional thieves start to feel the heat from the LAPD when they unknowingly leave a clue at their latest heist.",
        ReleaseYear = 1995,
        DurationMinutes = 170,
        RentalPrice = 42,
        PosterUrl = "/posters/heat.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=2GfZl4kuVNI",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "Platoon",
        Synopsis = "Chris Taylor, a neophyte recruit in Vietnam, finds himself caught in a battle of good and evil played out between two sergeants.",
        ReleaseYear = 1986,
        DurationMinutes = 120,
        RentalPrice = 39,
        PosterUrl = "/posters/platoon.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=R8weLPF4qBQ",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "20th Century Fox").Id
    },
    new Movie
    {
        Title = "Rambo: First Blood",
        Synopsis = "A troubled and misunderstood Vietnam veteran wanders into a small Oregon town looking for an old friend, but is harassed by the local sheriff and his deputies.",
        ReleaseYear = 1982,
        DurationMinutes = 93,
        RentalPrice = 39,
        PosterUrl = "/posters/rambo.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=oXngES_l_HQ",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Paramount Pictures").Id
    },
    new Movie
    {
        Title = "RoboCop",
        Synopsis = "In a dystopic and crime-ridden Detroit, a terminally wounded cop returns to the force as a powerful cyborg haunted by submerged memories.",
        ReleaseYear = 1987,
        DurationMinutes = 102,
        RentalPrice = 39,
        PosterUrl = "/posters/robocop.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=IqvRDhW-XVA",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "20th Century Fox").Id
    },
    new Movie
    {
        Title = "Braveheart",
        Synopsis = "Scottish warrior William Wallace leads his countrymen in a rebellion to free his homeland from the tyranny of King Edward I of England.",
        ReleaseYear = 1995,
        DurationMinutes = 178,
        RentalPrice = 45,
        PosterUrl = "/posters/braveheart.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=1NJO0jxBtMo",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Paramount Pictures").Id
    },
    new Movie
    {
        Title = "Interstellar",
        Synopsis = "When Earth becomes uninhabitable in the future, a farmer and ex-NASA pilot is tasked to pilot a spacecraft to find a new planet for humans.",
        ReleaseYear = 2014,
        DurationMinutes = 169,
        RentalPrice = 49,
        PosterUrl = "/posters/interstellar.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=zSWdZVtXT7E",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Paramount Pictures").Id
    },
    new Movie
    {
        Title = "Pirates of the Caribbean: The Curse of the Black Pearl",
        Synopsis = "Blacksmith Will Turner teams up with eccentric pirate Captain Jack Sparrow to save his love from Jack's former pirate allies who are now undead.",
        ReleaseYear = 2003,
        DurationMinutes = 143,
        RentalPrice = 42,
        PosterUrl = "/posters/pirates-of-the-caribbean.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=naQr0uTrH_s",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "Schindler's List",
        Synopsis = "In German-occupied Poland during World War II, industrialist Oskar Schindler gradually becomes concerned for his Jewish workforce after witnessing their persecution by the Nazis.",
        ReleaseYear = 1993,
        DurationMinutes = 195,
        RentalPrice = 45,
        PosterUrl = "/posters/schindlers-list.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=gG22XNhtnoY",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Universal Pictures").Id
    },
    new Movie
    {
        Title = "The Good, the Bad and the Ugly",
        Synopsis = "A bounty hunting scam joins two men in an uneasy alliance against a third in a race to find a fortune in gold buried in a remote cemetery.",
        ReleaseYear = 1966,
        DurationMinutes = 178,
        RentalPrice = 39,
        PosterUrl = "/posters/the-good-the-bad-and-the-ugly.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=WCN5JJY_wiA",
        Language = "Italian",
        Country = "Italy",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Universal Pictures").Id
    },
    new Movie
    {
        Title = "The Lord of the Rings: The Return of the King",
        Synopsis = "Gandalf and Aragorn lead the World of Men against Sauron's army to draw his gaze from Frodo and Sam as they approach Mount Doom with the One Ring.",
        ReleaseYear = 2003,
        DurationMinutes = 201,
        RentalPrice = 49,
        PosterUrl = "/posters/the-lord-of-the-rings-the-return-of-the-king.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=r5X-hFf6Bwo",
        Language = "English",
        Country = "New Zealand",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Warner Bros.").Id
    },
    new Movie
    {
        Title = "The Silence of the Lambs",
        Synopsis = "A young FBI cadet must receive the help of an incarcerated and manipulative cannibal killer to help catch another serial killer who skins his victims.",
        ReleaseYear = 1991,
        DurationMinutes = 118,
        RentalPrice = 42,
        PosterUrl = "/posters/the-silence-of-the-lambs.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=W6GDil0rGls",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "20th Century Fox").Id
    },
     new Movie
    {
        Title = "Cliffhanger",
        Synopsis = "A mountain climber is pitted against a group of violent criminals who have lost their cache of money in the Rocky Mountains.",
        ReleaseYear = 1993,
        DurationMinutes = 113,
        RentalPrice = 39,
        PosterUrl = "/posters/cliffhanger.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=c55DDQT8zDA",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "20th Century Fox").Id
    },
    new Movie
    {
        Title = "The Expendables",
        Synopsis = "A group of elite mercenaries are tasked with a mission to overthrow a Latin American dictator, but they soon discover darker forces at play.",
        ReleaseYear = 2010,
        DurationMinutes = 103,
        RentalPrice = 39,
        PosterUrl = "/posters/the-expendables.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=8KtYRALe-xo",
        Language = "English",
        Country = "USA",
        StockQuantity = 4,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "20th Century Fox").Id
    },
    new Movie
    {
        Title = "Indiana Jones and the Raiders of the Lost Ark",
        Synopsis = "Archaeologist and adventurer Indiana Jones is hired by the U.S. government to find the Ark of the Covenant before the Nazis can obtain its awesome powers.",
        ReleaseYear = 1981,
        DurationMinutes = 115,
        RentalPrice = 42,
        PosterUrl = "/posters/indiana-jones.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=Rh_BJXG1-44",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Paramount Pictures").Id
    },
    new Movie
    {
        Title = "Back to the Future",
        Synopsis = "Marty McFly, a 17-year-old high school student, is accidentally sent 30 years into the past in a time-traveling DeLorean invented by his close friend, the maverick scientist Doc Brown.",
        ReleaseYear = 1985,
        DurationMinutes = 116,
        RentalPrice = 42,
        PosterUrl = "/posters/back-to-the-future.jpg",
        TrailerUrl = "https://www.youtube.com/watch?v=qvsgGtivCgs",
        Language = "English",
        Country = "USA",
        StockQuantity = 5,
        IsFeatured = true,
        ProductionCompanyId = companies.First(c => c.Name == "Universal Pictures").Id
    }


};

        var existingMovieTitles = await context.Movies
            .Select(m => m.Title)
            .ToListAsync();

        var missingMovies = movies
            .Where(m => !existingMovieTitles.Contains(m.Title))
            .ToList();

        if (missingMovies.Count > 0)
        {
            context.Movies.AddRange(missingMovies);
            await context.SaveChangesAsync();
        }

        movies = await context.Movies.ToListAsync();

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
        var matrix = movies.First(m => m.Title == "The Matrix");
        var goodfellas = movies.First(m => m.Title == "Goodfellas");
        var terminator2 = movies.First(m => m.Title == "Terminator 2: Judgment Day");
        var fightclub = movies.First(m => m.Title == "Fight Club");
        var forrestgump = movies.First(m => m.Title == "Forrest Gump");
        var diehard = movies.First(m => m.Title == "Die Hard");
        var alien = movies.First(m => m.Title == "Alien");
        var predator = movies.First(m => m.Title == "Predator");
        var nightmare = movies.First(m => m.Title == "A Nightmare on Elm Street");
        var friday13 = movies.First(m => m.Title == "Friday the 13th");
        var gladiator = movies.First(m => m.Title == "Gladiator");
        var halloween = movies.First(m => m.Title == "Halloween");
        var hellraiser = movies.First(m => m.Title == "Hellraiser");
        var inception = movies.First(m => m.Title == "Inception");
        var jurassicpark = movies.First(m => m.Title == "Jurassic Park");
        var rocky = movies.First(m => m.Title == "Rocky");
        var savingprivateryan = movies.First(m => m.Title == "Saving Private Ryan");
        var darkknight = movies.First(m => m.Title == "The Dark Knight");
        var exorcist = movies.First(m => m.Title == "The Exorcist");
        var shining = movies.First(m => m.Title == "The Shining");
        var texaschainsaw = movies.First(m => m.Title == "The Texas Chain Saw Massacre");
        var grantorino = movies.First(m => m.Title == "Gran Torino");
        var castaway = movies.First(m => m.Title == "Cast Away");
        var jaws = movies.First(m => m.Title == "Jaws");
        var se7en = movies.First(m => m.Title == "Se7en");
        var americanhistoryx = movies.First(m => m.Title == "American History X");
        var taxidriver = movies.First(m => m.Title == "Taxi Driver");
        var casino = movies.First(m => m.Title == "Casino");
        var nocountry = movies.First(m => m.Title == "No Country for Old Men");
        var themask = movies.First(m => m.Title == "The Mask");
        var thething = movies.First(m => m.Title == "The Thing");
        var bloodsport = movies.First(m => m.Title == "Bloodsport");
        var bigtrouble = movies.First(m => m.Title == "Big Trouble in Little China");
        var heat = movies.First(m => m.Title == "Heat");
        var platoon = movies.First(m => m.Title == "Platoon");
        var rambo = movies.First(m => m.Title == "Rambo: First Blood");
        var robocop = movies.First(m => m.Title == "RoboCop");
        var braveheart = movies.First(m => m.Title == "Braveheart");
        var interstellar = movies.First(m => m.Title == "Interstellar");
        var pirates = movies.First(m => m.Title == "Pirates of the Caribbean: The Curse of the Black Pearl");
        var schindlerslist = movies.First(m => m.Title == "Schindler's List");
        var goodbadugly = movies.First(m => m.Title == "The Good, the Bad and the Ugly");
        var lotr = movies.First(m => m.Title == "The Lord of the Rings: The Return of the King");
        var silencelambs = movies.First(m => m.Title == "The Silence of the Lambs");
        var cliffhanger = movies.First(m => m.Title == "Cliffhanger");
        var expendables = movies.First(m => m.Title == "The Expendables");
        var indianajones = movies.First(m => m.Title == "Indiana Jones and the Raiders of the Lost Ark");
        var backtothefuture = movies.First(m => m.Title == "Back to the Future");

        var action = genres.First(g => g.Name == "Action");
        var drama = genres.First(g => g.Name == "Drama");
        var crime = genres.First(g => g.Name == "Crime");
        var scifi = genres.First(g => g.Name == "Sci-Fi");
        var romance = genres.First(g => g.Name == "Romance");
        var thriller = genres.First(g => g.Name == "Thriller");
        var adventure = genres.First(g => g.Name == "Adventure");
        var war = genres.First(g => g.Name == "War");
        var horror = genres.First(g => g.Name == "Horror");
        var comedy = genres.First(g => g.Name == "Comedy");

        var movieGenres = new List<MovieGenre>
        {   
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
            new MovieGenre { MovieId = titanic.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = matrix.Id, GenreId = action.Id },
            new MovieGenre { MovieId = matrix.Id, GenreId = scifi.Id },
            new MovieGenre { MovieId = goodfellas.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = goodfellas.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = terminator2.Id, GenreId = action.Id },
            new MovieGenre { MovieId = terminator2.Id, GenreId = scifi.Id },
            new MovieGenre { MovieId = fightclub.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = fightclub.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = forrestgump.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = forrestgump.Id, GenreId = romance.Id },
            new MovieGenre { MovieId = diehard.Id, GenreId = action.Id },
            new MovieGenre { MovieId = diehard.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = alien.Id, GenreId = scifi.Id },
            new MovieGenre { MovieId = alien.Id, GenreId = genres.First(g => g.Name == "Horror").Id },
            new MovieGenre { MovieId = predator.Id, GenreId = action.Id },
            new MovieGenre { MovieId = predator.Id, GenreId = scifi.Id },
            new MovieGenre { MovieId = nightmare.Id, GenreId = horror.Id },
            new MovieGenre { MovieId = friday13.Id, GenreId = horror.Id },
            new MovieGenre { MovieId = gladiator.Id, GenreId = action.Id },
            new MovieGenre { MovieId = gladiator.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = gladiator.Id, GenreId = adventure.Id },
            new MovieGenre { MovieId = halloween.Id, GenreId = horror.Id },
            new MovieGenre { MovieId = halloween.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = hellraiser.Id, GenreId = horror.Id },
            new MovieGenre { MovieId = inception.Id, GenreId = scifi.Id },
            new MovieGenre { MovieId = inception.Id, GenreId = action.Id },
            new MovieGenre { MovieId = inception.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = jurassicpark.Id, GenreId = scifi.Id },
            new MovieGenre { MovieId = jurassicpark.Id, GenreId = adventure.Id },
            new MovieGenre { MovieId = rocky.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = rocky.Id, GenreId = action.Id },
            new MovieGenre { MovieId = savingprivateryan.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = savingprivateryan.Id, GenreId = war.Id },
            new MovieGenre { MovieId = darkknight.Id, GenreId = action.Id },
            new MovieGenre { MovieId = darkknight.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = darkknight.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = exorcist.Id, GenreId = horror.Id },
            new MovieGenre { MovieId = shining.Id, GenreId = horror.Id },
            new MovieGenre { MovieId = shining.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = texaschainsaw.Id, GenreId = horror.Id },
            new MovieGenre { MovieId = grantorino.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = castaway.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = castaway.Id, GenreId = adventure.Id },
            new MovieGenre { MovieId = jaws.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = jaws.Id, GenreId = adventure.Id },
            new MovieGenre { MovieId = se7en.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = se7en.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = americanhistoryx.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = americanhistoryx.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = taxidriver.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = taxidriver.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = taxidriver.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = casino.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = casino.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = nocountry.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = nocountry.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = themask.Id, GenreId = comedy.Id },
            new MovieGenre { MovieId = themask.Id, GenreId = action.Id },
            new MovieGenre { MovieId = thething.Id, GenreId = horror.Id },
            new MovieGenre { MovieId = thething.Id, GenreId = scifi.Id },
            new MovieGenre { MovieId = bloodsport.Id, GenreId = action.Id },
            new MovieGenre { MovieId = bigtrouble.Id, GenreId = action.Id },
            new MovieGenre { MovieId = bigtrouble.Id, GenreId = comedy.Id },
            new MovieGenre { MovieId = bigtrouble.Id, GenreId = adventure.Id },
            new MovieGenre { MovieId = heat.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = heat.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = heat.Id, GenreId = action.Id },
            new MovieGenre { MovieId = platoon.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = platoon.Id, GenreId = war.Id },
            new MovieGenre { MovieId = rambo.Id, GenreId = action.Id },
            new MovieGenre { MovieId = rambo.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = robocop.Id, GenreId = action.Id },
            new MovieGenre { MovieId = robocop.Id, GenreId = scifi.Id },
            new MovieGenre { MovieId = robocop.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = braveheart.Id, GenreId = action.Id },
            new MovieGenre { MovieId = braveheart.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = braveheart.Id, GenreId = war.Id },
            new MovieGenre { MovieId = interstellar.Id, GenreId = scifi.Id },
            new MovieGenre { MovieId = interstellar.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = interstellar.Id, GenreId = adventure.Id },
            new MovieGenre { MovieId = pirates.Id, GenreId = action.Id },
            new MovieGenre { MovieId = pirates.Id, GenreId = adventure.Id },
            new MovieGenre { MovieId = pirates.Id, GenreId = genres.First(g => g.Name == "Fantasy").Id },
            new MovieGenre { MovieId = schindlerslist.Id, GenreId = drama.Id },
            new MovieGenre { MovieId = schindlerslist.Id, GenreId = war.Id },
            new MovieGenre { MovieId = goodbadugly.Id, GenreId = genres.First(g => g.Name == "Western").Id },
            new MovieGenre { MovieId = goodbadugly.Id, GenreId = adventure.Id },
            new MovieGenre { MovieId = lotr.Id, GenreId = genres.First(g => g.Name == "Fantasy").Id },
            new MovieGenre { MovieId = lotr.Id, GenreId = adventure.Id },
            new MovieGenre { MovieId = lotr.Id, GenreId = action.Id },
            new MovieGenre { MovieId = silencelambs.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = silencelambs.Id, GenreId = horror.Id },
            new MovieGenre { MovieId = silencelambs.Id, GenreId = crime.Id },
            new MovieGenre { MovieId = cliffhanger.Id, GenreId = action.Id },
            new MovieGenre { MovieId = cliffhanger.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = cliffhanger.Id, GenreId = adventure.Id },
            new MovieGenre { MovieId = expendables.Id, GenreId = action.Id },
            new MovieGenre { MovieId = expendables.Id, GenreId = thriller.Id },
            new MovieGenre { MovieId = indianajones.Id, GenreId = action.Id },
            new MovieGenre { MovieId = indianajones.Id, GenreId = adventure.Id },
            new MovieGenre { MovieId = backtothefuture.Id, GenreId = scifi.Id },
            new MovieGenre { MovieId = backtothefuture.Id, GenreId = comedy.Id },
            new MovieGenre { MovieId = backtothefuture.Id, GenreId = adventure.Id }

        };

        var existingMovieGenreKeys = await context.MovieGenres
            .Select(mg => $"{mg.MovieId}-{mg.GenreId}")
            .ToListAsync();

        var missingMovieGenres = movieGenres
            .Where(mg => !existingMovieGenreKeys.Contains($"{mg.MovieId}-{mg.GenreId}"))
            .ToList();

        if (missingMovieGenres.Count > 0)
        {
            context.MovieGenres.AddRange(missingMovieGenres);
            await context.SaveChangesAsync();
        }

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
    new Person { FullName = "Kate Winslet", Country = "UK", Bio = "Known for Titanic." },
    new Person { FullName = "Lana Wachowski", Country = "USA", Bio = "Director of The Matrix trilogy." },
    new Person { FullName = "Keanu Reeves", Country = "Canada", Bio = "Known for The Matrix and John Wick." },
    new Person { FullName = "Laurence Fishburne", Country = "USA", Bio = "Known for The Matrix." },
    new Person { FullName = "Martin Scorsese", Country = "USA", Bio = "Director of Goodfellas and The Departed." },
    new Person { FullName = "Ray Liotta", Country = "USA", Bio = "Known for Goodfellas." },
    new Person { FullName = "Robert De Niro", Country = "USA", Bio = "Known for Goodfellas and Taxi Driver." },
    new Person { FullName = "Arnold Schwarzenegger", Country = "Austria", Bio = "Known for Terminator and Predator." },
    new Person { FullName = "Linda Hamilton", Country = "USA", Bio = "Known for Terminator." },
    new Person { FullName = "David Fincher", Country = "USA", Bio = "Director of Fight Club and Se7en." },
    new Person { FullName = "Brad Pitt", Country = "USA", Bio = "Known for Fight Club and Once Upon a Time in Hollywood." },
    new Person { FullName = "Edward Norton", Country = "USA", Bio = "Known for Fight Club." },
    new Person { FullName = "Robert Zemeckis", Country = "USA", Bio = "Director of Forrest Gump and Back to the Future." },
    new Person { FullName = "Tom Hanks", Country = "USA", Bio = "Known for Forrest Gump and Cast Away." },
    new Person { FullName = "John McTiernan", Country = "USA", Bio = "Director of Die Hard and Predator." },
    new Person { FullName = "Bruce Willis", Country = "USA", Bio = "Known for Die Hard." },
    new Person { FullName = "Ridley Scott", Country = "UK", Bio = "Director of Alien and Blade Runner." },
    new Person { FullName = "Sigourney Weaver", Country = "USA", Bio = "Known for Alien." },
    new Person { FullName = "Wes Craven", Country = "USA", Bio = "Director of A Nightmare on Elm Street and Scream." },
    new Person { FullName = "Robert Englund", Country = "USA", Bio = "Known for playing Freddy Krueger." },
    new Person { FullName = "Sean S. Cunningham", Country = "USA", Bio = "Director of Friday the 13th." },
    new Person { FullName = "Betsy Palmer", Country = "USA", Bio = "Known for Friday the 13th." },
    new Person { FullName = "Russell Crowe", Country = "New Zealand", Bio = "Known for Gladiator and A Beautiful Mind." },
    new Person { FullName = "Joaquin Phoenix", Country = "USA", Bio = "Known for Gladiator and Joker." },
    new Person { FullName = "John Carpenter", Country = "USA", Bio = "Director of Halloween and The Thing." },
    new Person { FullName = "Jamie Lee Curtis", Country = "USA", Bio = "Known for Halloween." },
    new Person { FullName = "Clive Barker", Country = "UK", Bio = "Director of Hellraiser." },
    new Person { FullName = "Doug Bradley", Country = "UK", Bio = "Known for playing Pinhead in Hellraiser." },
    new Person { FullName = "Christopher Nolan", Country = "UK", Bio = "Director of Inception and The Dark Knight." },
    new Person { FullName = "Steven Spielberg", Country = "USA", Bio = "Director of Jurassic Park and Saving Private Ryan." },
    new Person { FullName = "Sam Neill", Country = "Northern Ireland", Bio = "Known for Jurassic Park." },
    new Person { FullName = "Laura Dern", Country = "USA", Bio = "Known for Jurassic Park." },
    new Person { FullName = "John G. Avildsen", Country = "USA", Bio = "Director of Rocky." },
    new Person { FullName = "Sylvester Stallone", Country = "USA", Bio = "Known for Rocky and Rambo." },
    new Person { FullName = "Matt Damon", Country = "USA", Bio = "Known for Saving Private Ryan and The Bourne Identity." },
    new Person { FullName = "Christian Bale", Country = "UK", Bio = "Known for The Dark Knight and American Psycho." },
    new Person { FullName = "Heath Ledger", Country = "Australia", Bio = "Known for The Dark Knight." },
    new Person { FullName = "William Friedkin", Country = "USA", Bio = "Director of The Exorcist." },
    new Person { FullName = "Linda Blair", Country = "USA", Bio = "Known for The Exorcist." },
    new Person { FullName = "Stanley Kubrick", Country = "USA", Bio = "Director of The Shining and 2001: A Space Odyssey." },
    new Person { FullName = "Jack Nicholson", Country = "USA", Bio = "Known for The Shining and One Flew Over the Cuckoo's Nest." },
    new Person { FullName = "Tobe Hooper", Country = "USA", Bio = "Director of The Texas Chain Saw Massacre." },
    new Person { FullName = "Clint Eastwood", Country = "USA", Bio = "Director of Gran Torino and Unforgiven." },
    new Person { FullName = "Robert Zemeckis", Country = "USA", Bio = "Director of Cast Away and Forrest Gump." },
    new Person { FullName = "Roy Scheider", Country = "USA", Bio = "Known for Jaws." },
    new Person { FullName = "Morgan Freeman", Country = "USA", Bio = "Known for Se7en and The Shawshank Redemption." },
    new Person { FullName = "Edward Norton", Country = "USA", Bio = "Known for American History X and Fight Club." },
    new Person { FullName = "Tony Kaye", Country = "UK", Bio = "Director of American History X." },
    new Person { FullName = "Robert De Niro", Country = "USA", Bio = "Known for Taxi Driver and Goodfellas." },
    new Person { FullName = "Joe Pesci", Country = "USA", Bio = "Known for Casino and Goodfellas." },
    new Person { FullName = "Joel Coen", Country = "USA", Bio = "Director of No Country for Old Men and Fargo." },
    new Person { FullName = "Javier Bardem", Country = "Spain", Bio = "Known for No Country for Old Men." },
    new Person { FullName = "Chuck Russell", Country = "USA", Bio = "Director of The Mask." },
    new Person { FullName = "Jim Carrey", Country = "Canada", Bio = "Known for The Mask and Ace Ventura." },
    new Person { FullName = "Kurt Russell", Country = "USA", Bio = "Known for The Thing and Escape from New York." },
    new Person { FullName = "Newt Arnold", Country = "USA", Bio = "Director of Bloodsport." },
    new Person { FullName = "Jean-Claude Van Damme", Country = "Belgium", Bio = "Known for Bloodsport and Kickboxer." },
    new Person { FullName = "Kim Cattrall", Country = "UK", Bio = "Known for Big Trouble in Little China." },
    new Person { FullName = "Michael Mann", Country = "USA", Bio = "Director of Heat and Collateral." },
    new Person { FullName = "Val Kilmer", Country = "USA", Bio = "Known for Heat and Top Gun." },
    new Person { FullName = "Oliver Stone", Country = "USA", Bio = "Director of Platoon and Wall Street." },
    new Person { FullName = "Charlie Sheen", Country = "USA", Bio = "Known for Platoon and Two and a Half Men." },
    new Person { FullName = "Willem Dafoe", Country = "USA", Bio = "Known for Platoon and Spider-Man." },
    new Person { FullName = "Ted Kotcheff", Country = "Canada", Bio = "Director of Rambo: First Blood." },
    new Person { FullName = "Paul Verhoeven", Country = "Netherlands", Bio = "Director of RoboCop and Total Recall." },
    new Person { FullName = "Peter Weller", Country = "USA", Bio = "Known for RoboCop." },
    new Person { FullName = "Mel Gibson", Country = "USA", Bio = "Director of Braveheart and known for Mad Max." },
    new Person { FullName = "Sophie Marceau", Country = "France", Bio = "Known for Braveheart." },
    new Person { FullName = "Matthew McConaughey", Country = "USA", Bio = "Known for Interstellar and Dallas Buyers Club." },
    new Person { FullName = "Anne Hathaway", Country = "USA", Bio = "Known for Interstellar and The Dark Knight Rises." },
    new Person { FullName = "Gore Verbinski", Country = "USA", Bio = "Director of Pirates of the Caribbean." },
    new Person { FullName = "Johnny Depp", Country = "USA", Bio = "Known for Pirates of the Caribbean." },
    new Person { FullName = "Orlando Bloom", Country = "UK", Bio = "Known for Pirates of the Caribbean and Lord of the Rings." },
    new Person { FullName = "Liam Neeson", Country = "Northern Ireland", Bio = "Known for Schindler's List and Taken." },
    new Person { FullName = "Ralph Fiennes", Country = "UK", Bio = "Known for Schindler's List and Harry Potter." },
    new Person { FullName = "Sergio Leone", Country = "Italy", Bio = "Director of The Good, the Bad and the Ugly." },
    new Person { FullName = "Eli Wallach", Country = "USA", Bio = "Known for The Good, the Bad and the Ugly." },
    new Person { FullName = "Peter Jackson", Country = "New Zealand", Bio = "Director of The Lord of the Rings trilogy." },
    new Person { FullName = "Elijah Wood", Country = "USA", Bio = "Known for The Lord of the Rings." },
    new Person { FullName = "Viggo Mortensen", Country = "USA", Bio = "Known for The Lord of the Rings." },
    new Person { FullName = "Jonathan Demme", Country = "USA", Bio = "Director of The Silence of the Lambs." },
    new Person { FullName = "Anthony Hopkins", Country = "UK", Bio = "Known for The Silence of the Lambs." },
    new Person { FullName = "Jodie Foster", Country = "USA", Bio = "Known for The Silence of the Lambs and Taxi Driver." },
    new Person { FullName = "Renny Harlin", Country = "Finland", Bio = "Director of Cliffhanger and Die Hard 2." },
    new Person { FullName = "John Lithgow", Country = "USA", Bio = "Known for Cliffhanger and Dexter." },
    new Person { FullName = "Jason Statham", Country = "UK", Bio = "Known for The Expendables and The Transporter." },
    new Person { FullName = "Harrison Ford", Country = "USA", Bio = "Known for Indiana Jones and Star Wars." },
    new Person { FullName = "Karen Allen", Country = "USA", Bio = "Known for Raiders of the Lost Ark." },
    new Person { FullName = "Michael J. Fox", Country = "Canada", Bio = "Known for Back to the Future." },
    new Person { FullName = "Christopher Lloyd", Country = "USA", Bio = "Known for Back to the Future." }
};

        var existingPersonNames = await context.Persons
            .Select(p => p.FullName)
            .ToListAsync();

        var missingPersons = persons
            .Where(p => !existingPersonNames.Contains(p.FullName))
            .ToList();

        if (missingPersons.Count > 0)
        {
            context.Persons.AddRange(missingPersons);
            await context.SaveChangesAsync();
        }

        persons = await context.Persons.ToListAsync();


        // =========================
        // Seed: MovieCredits
        // =========================
        var movieCredits = new List<MovieCredit>
        {
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
            new MovieCredit { MovieId = titanic.Id, PersonId = persons.First(p => p.FullName == "Kate Winslet").Id, Role = CreditRole.Actor, CharacterName = "Rose DeWitt Bukater", DisplayOrder = 3 },

            // The Matrix
            new MovieCredit { MovieId = matrix.Id, PersonId = persons.First(p => p.FullName == "Lana Wachowski").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = matrix.Id, PersonId = persons.First(p => p.FullName == "Keanu Reeves").Id, Role = CreditRole.Actor, CharacterName = "Neo", DisplayOrder = 2 },
            new MovieCredit { MovieId = matrix.Id, PersonId = persons.First(p => p.FullName == "Laurence Fishburne").Id, Role = CreditRole.Actor, CharacterName = "Morpheus", DisplayOrder = 3 },

            // Goodfellas
            new MovieCredit { MovieId = goodfellas.Id, PersonId = persons.First(p => p.FullName == "Martin Scorsese").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = goodfellas.Id, PersonId = persons.First(p => p.FullName == "Ray Liotta").Id, Role = CreditRole.Actor, CharacterName = "Henry Hill", DisplayOrder = 2 },
            new MovieCredit { MovieId = goodfellas.Id, PersonId = persons.First(p => p.FullName == "Robert De Niro").Id, Role = CreditRole.Actor, CharacterName = "Jimmy Conway", DisplayOrder = 3 },

            // Terminator 2
            new MovieCredit { MovieId = terminator2.Id, PersonId = persons.First(p => p.FullName == "James Cameron").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = terminator2.Id, PersonId = persons.First(p => p.FullName == "Arnold Schwarzenegger").Id, Role = CreditRole.Actor, CharacterName = "T-800", DisplayOrder = 2 },
            new MovieCredit { MovieId = terminator2.Id, PersonId = persons.First(p => p.FullName == "Linda Hamilton").Id, Role = CreditRole.Actor, CharacterName = "Sarah Connor", DisplayOrder = 3 },

            // Fight Club
            new MovieCredit { MovieId = fightclub.Id, PersonId = persons.First(p => p.FullName == "David Fincher").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = fightclub.Id, PersonId = persons.First(p => p.FullName == "Brad Pitt").Id, Role = CreditRole.Actor, CharacterName = "Tyler Durden", DisplayOrder = 2 },
            new MovieCredit { MovieId = fightclub.Id, PersonId = persons.First(p => p.FullName == "Edward Norton").Id, Role = CreditRole.Actor, CharacterName = "The Narrator", DisplayOrder = 3 },

            // Forrest Gump
            new MovieCredit { MovieId = forrestgump.Id, PersonId = persons.First(p => p.FullName == "Robert Zemeckis").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = forrestgump.Id, PersonId = persons.First(p => p.FullName == "Tom Hanks").Id, Role = CreditRole.Actor, CharacterName = "Forrest Gump", DisplayOrder = 2 },

            // Die Hard
            new MovieCredit { MovieId = diehard.Id, PersonId = persons.First(p => p.FullName == "John McTiernan").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = diehard.Id, PersonId = persons.First(p => p.FullName == "Bruce Willis").Id, Role = CreditRole.Actor, CharacterName = "John McClane", DisplayOrder = 2 },

            // Alien
            new MovieCredit { MovieId = alien.Id, PersonId = persons.First(p => p.FullName == "Ridley Scott").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = alien.Id, PersonId = persons.First(p => p.FullName == "Sigourney Weaver").Id, Role = CreditRole.Actor, CharacterName = "Ellen Ripley", DisplayOrder = 2 },

            // Predator
            new MovieCredit { MovieId = predator.Id, PersonId = persons.First(p => p.FullName == "John McTiernan").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = predator.Id, PersonId = persons.First(p => p.FullName == "Arnold Schwarzenegger").Id, Role = CreditRole.Actor, CharacterName = "Dutch", DisplayOrder = 2 },

            // A Nightmare on Elm Street
            new MovieCredit { MovieId = nightmare.Id, PersonId = persons.First(p => p.FullName == "Wes Craven").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = nightmare.Id, PersonId = persons.First(p => p.FullName == "Robert Englund").Id, Role = CreditRole.Actor, CharacterName = "Freddy Krueger", DisplayOrder = 2 },

            // Friday the 13th
            new MovieCredit { MovieId = friday13.Id, PersonId = persons.First(p => p.FullName == "Sean S. Cunningham").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = friday13.Id, PersonId = persons.First(p => p.FullName == "Betsy Palmer").Id, Role = CreditRole.Actor, CharacterName = "Mrs. Voorhees", DisplayOrder = 2 },

            // Gladiator
            new MovieCredit { MovieId = gladiator.Id, PersonId = persons.First(p => p.FullName == "Ridley Scott").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = gladiator.Id, PersonId = persons.First(p => p.FullName == "Russell Crowe").Id, Role = CreditRole.Actor, CharacterName = "Maximus", DisplayOrder = 2 },
            new MovieCredit { MovieId = gladiator.Id, PersonId = persons.First(p => p.FullName == "Joaquin Phoenix").Id, Role = CreditRole.Actor, CharacterName = "Commodus", DisplayOrder = 3 },

            // Halloween
            new MovieCredit { MovieId = halloween.Id, PersonId = persons.First(p => p.FullName == "John Carpenter").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = halloween.Id, PersonId = persons.First(p => p.FullName == "Jamie Lee Curtis").Id, Role = CreditRole.Actor, CharacterName = "Laurie Strode", DisplayOrder = 2 },

            // Hellraiser
            new MovieCredit { MovieId = hellraiser.Id, PersonId = persons.First(p => p.FullName == "Clive Barker").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = hellraiser.Id, PersonId = persons.First(p => p.FullName == "Doug Bradley").Id, Role = CreditRole.Actor, CharacterName = "Pinhead", DisplayOrder = 2 },

            // Inception
            new MovieCredit { MovieId = inception.Id, PersonId = persons.First(p => p.FullName == "Christopher Nolan").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = inception.Id, PersonId = persons.First(p => p.FullName == "Leonardo DiCaprio").Id, Role = CreditRole.Actor, CharacterName = "Dom Cobb", DisplayOrder = 2 },

            // Jurassic Park
            new MovieCredit { MovieId = jurassicpark.Id, PersonId = persons.First(p => p.FullName == "Steven Spielberg").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = jurassicpark.Id, PersonId = persons.First(p => p.FullName == "Sam Neill").Id, Role = CreditRole.Actor, CharacterName = "Dr. Alan Grant", DisplayOrder = 2 },
            new MovieCredit { MovieId = jurassicpark.Id, PersonId = persons.First(p => p.FullName == "Laura Dern").Id, Role = CreditRole.Actor, CharacterName = "Dr. Ellie Sattler", DisplayOrder = 3 },

            // Rocky
            new MovieCredit { MovieId = rocky.Id, PersonId = persons.First(p => p.FullName == "John G. Avildsen").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = rocky.Id, PersonId = persons.First(p => p.FullName == "Sylvester Stallone").Id, Role = CreditRole.Actor, CharacterName = "Rocky Balboa", DisplayOrder = 2 },

            // Saving Private Ryan
            new MovieCredit { MovieId = savingprivateryan.Id, PersonId = persons.First(p => p.FullName == "Steven Spielberg").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = savingprivateryan.Id, PersonId = persons.First(p => p.FullName == "Tom Hanks").Id, Role = CreditRole.Actor, CharacterName = "Captain Miller", DisplayOrder = 2 },
            new MovieCredit { MovieId = savingprivateryan.Id, PersonId = persons.First(p => p.FullName == "Matt Damon").Id, Role = CreditRole.Actor, CharacterName = "Private Ryan", DisplayOrder = 3 },

            // The Dark Knight
            new MovieCredit { MovieId = darkknight.Id, PersonId = persons.First(p => p.FullName == "Christopher Nolan").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = darkknight.Id, PersonId = persons.First(p => p.FullName == "Christian Bale").Id, Role = CreditRole.Actor, CharacterName = "Bruce Wayne", DisplayOrder = 2 },
            new MovieCredit { MovieId = darkknight.Id, PersonId = persons.First(p => p.FullName == "Heath Ledger").Id, Role = CreditRole.Actor, CharacterName = "The Joker", DisplayOrder = 3 },

            // The Exorcist
            new MovieCredit { MovieId = exorcist.Id, PersonId = persons.First(p => p.FullName == "William Friedkin").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = exorcist.Id, PersonId = persons.First(p => p.FullName == "Linda Blair").Id, Role = CreditRole.Actor, CharacterName = "Regan", DisplayOrder = 2 },

            // The Shining
            new MovieCredit { MovieId = shining.Id, PersonId = persons.First(p => p.FullName == "Stanley Kubrick").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = shining.Id, PersonId = persons.First(p => p.FullName == "Jack Nicholson").Id, Role = CreditRole.Actor, CharacterName = "Jack Torrance", DisplayOrder = 2 },

            // The Texas Chain Saw Massacre
            new MovieCredit { MovieId = texaschainsaw.Id, PersonId = persons.First(p => p.FullName == "Tobe Hooper").Id, Role = CreditRole.Director, DisplayOrder = 1 },

            // Gran Torino
            new MovieCredit { MovieId = grantorino.Id, PersonId = persons.First(p => p.FullName == "Clint Eastwood").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = grantorino.Id, PersonId = persons.First(p => p.FullName == "Clint Eastwood").Id, Role = CreditRole.Actor, CharacterName = "Walt Kowalski", DisplayOrder = 2 },

            // Cast Away
            new MovieCredit { MovieId = castaway.Id, PersonId = persons.First(p => p.FullName == "Robert Zemeckis").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = castaway.Id, PersonId = persons.First(p => p.FullName == "Tom Hanks").Id, Role = CreditRole.Actor, CharacterName = "Chuck Noland", DisplayOrder = 2 },

            // Jaws
            new MovieCredit { MovieId = jaws.Id, PersonId = persons.First(p => p.FullName == "Steven Spielberg").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = jaws.Id, PersonId = persons.First(p => p.FullName == "Roy Scheider").Id, Role = CreditRole.Actor, CharacterName = "Chief Brody", DisplayOrder = 2 },

            // Se7en
            new MovieCredit { MovieId = se7en.Id, PersonId = persons.First(p => p.FullName == "David Fincher").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = se7en.Id, PersonId = persons.First(p => p.FullName == "Brad Pitt").Id, Role = CreditRole.Actor, CharacterName = "Detective Mills", DisplayOrder = 2 },
            new MovieCredit { MovieId = se7en.Id, PersonId = persons.First(p => p.FullName == "Morgan Freeman").Id, Role = CreditRole.Actor, CharacterName = "Detective Somerset", DisplayOrder = 3 },

            // American History X
            new MovieCredit { MovieId = americanhistoryx.Id, PersonId = persons.First(p => p.FullName == "Tony Kaye").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = americanhistoryx.Id, PersonId = persons.First(p => p.FullName == "Edward Norton").Id, Role = CreditRole.Actor, CharacterName = "Derek Vinyard", DisplayOrder = 2 },

            // Taxi Driver
            new MovieCredit { MovieId = taxidriver.Id, PersonId = persons.First(p => p.FullName == "Martin Scorsese").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = taxidriver.Id, PersonId = persons.First(p => p.FullName == "Robert De Niro").Id, Role = CreditRole.Actor, CharacterName = "Travis Bickle", DisplayOrder = 2 },

            // Casino
            new MovieCredit { MovieId = casino.Id, PersonId = persons.First(p => p.FullName == "Martin Scorsese").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = casino.Id, PersonId = persons.First(p => p.FullName == "Robert De Niro").Id, Role = CreditRole.Actor, CharacterName = "Sam Rothstein", DisplayOrder = 2 },
            new MovieCredit { MovieId = casino.Id, PersonId = persons.First(p => p.FullName == "Joe Pesci").Id, Role = CreditRole.Actor, CharacterName = "Nicky Santoro", DisplayOrder = 3 },

            // No Country for Old Men
            new MovieCredit { MovieId = nocountry.Id, PersonId = persons.First(p => p.FullName == "Joel Coen").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = nocountry.Id, PersonId = persons.First(p => p.FullName == "Javier Bardem").Id, Role = CreditRole.Actor, CharacterName = "Anton Chigurh", DisplayOrder = 2 },

            // The Mask
            new MovieCredit { MovieId = themask.Id, PersonId = persons.First(p => p.FullName == "Chuck Russell").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = themask.Id, PersonId = persons.First(p => p.FullName == "Jim Carrey").Id, Role = CreditRole.Actor, CharacterName = "Stanley Ipkiss", DisplayOrder = 2 },

            // The Thing
            new MovieCredit { MovieId = thething.Id, PersonId = persons.First(p => p.FullName == "John Carpenter").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = thething.Id, PersonId = persons.First(p => p.FullName == "Kurt Russell").Id, Role = CreditRole.Actor, CharacterName = "MacReady", DisplayOrder = 2 },

            // Bloodsport
            new MovieCredit { MovieId = bloodsport.Id, PersonId = persons.First(p => p.FullName == "Newt Arnold").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = bloodsport.Id, PersonId = persons.First(p => p.FullName == "Jean-Claude Van Damme").Id, Role = CreditRole.Actor, CharacterName = "Frank Dux", DisplayOrder = 2 },

            // Big Trouble in Little China
            new MovieCredit { MovieId = bigtrouble.Id, PersonId = persons.First(p => p.FullName == "John Carpenter").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = bigtrouble.Id, PersonId = persons.First(p => p.FullName == "Kurt Russell").Id, Role = CreditRole.Actor, CharacterName = "Jack Burton", DisplayOrder = 2 },
            new MovieCredit { MovieId = bigtrouble.Id, PersonId = persons.First(p => p.FullName == "Kim Cattrall").Id, Role = CreditRole.Actor, CharacterName = "Gracie Law", DisplayOrder = 3 },

            // Heat
            new MovieCredit { MovieId = heat.Id, PersonId = persons.First(p => p.FullName == "Michael Mann").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = heat.Id, PersonId = persons.First(p => p.FullName == "Al Pacino").Id, Role = CreditRole.Actor, CharacterName = "Vincent Hanna", DisplayOrder = 2 },
            new MovieCredit { MovieId = heat.Id, PersonId = persons.First(p => p.FullName == "Robert De Niro").Id, Role = CreditRole.Actor, CharacterName = "Neil McCauley", DisplayOrder = 3 },
            new MovieCredit { MovieId = heat.Id, PersonId = persons.First(p => p.FullName == "Val Kilmer").Id, Role = CreditRole.Actor, CharacterName = "Chris Shiherlis", DisplayOrder = 4 },

            // Platoon
            new MovieCredit { MovieId = platoon.Id, PersonId = persons.First(p => p.FullName == "Oliver Stone").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = platoon.Id, PersonId = persons.First(p => p.FullName == "Charlie Sheen").Id, Role = CreditRole.Actor, CharacterName = "Chris Taylor", DisplayOrder = 2 },
            new MovieCredit { MovieId = platoon.Id, PersonId = persons.First(p => p.FullName == "Willem Dafoe").Id, Role = CreditRole.Actor, CharacterName = "Sgt. Elias", DisplayOrder = 3 },

            // Rambo: First Blood
            new MovieCredit { MovieId = rambo.Id, PersonId = persons.First(p => p.FullName == "Ted Kotcheff").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = rambo.Id, PersonId = persons.First(p => p.FullName == "Sylvester Stallone").Id, Role = CreditRole.Actor, CharacterName = "John Rambo", DisplayOrder = 2 },

            // RoboCop
            new MovieCredit { MovieId = robocop.Id, PersonId = persons.First(p => p.FullName == "Paul Verhoeven").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = robocop.Id, PersonId = persons.First(p => p.FullName == "Peter Weller").Id, Role = CreditRole.Actor, CharacterName = "Alex Murphy / RoboCop", DisplayOrder = 2 },

            // Braveheart
            new MovieCredit { MovieId = braveheart.Id, PersonId = persons.First(p => p.FullName == "Mel Gibson").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = braveheart.Id, PersonId = persons.First(p => p.FullName == "Mel Gibson").Id, Role = CreditRole.Actor, CharacterName = "William Wallace", DisplayOrder = 2 },
            new MovieCredit { MovieId = braveheart.Id, PersonId = persons.First(p => p.FullName == "Sophie Marceau").Id, Role = CreditRole.Actor, CharacterName = "Princess Isabelle", DisplayOrder = 3 },

            // Interstellar
            new MovieCredit { MovieId = interstellar.Id, PersonId = persons.First(p => p.FullName == "Christopher Nolan").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = interstellar.Id, PersonId = persons.First(p => p.FullName == "Matthew McConaughey").Id, Role = CreditRole.Actor, CharacterName = "Cooper", DisplayOrder = 2 },
            new MovieCredit { MovieId = interstellar.Id, PersonId = persons.First(p => p.FullName == "Anne Hathaway").Id, Role = CreditRole.Actor, CharacterName = "Dr. Brand", DisplayOrder = 3 },

            // Pirates of the Caribbean
            new MovieCredit { MovieId = pirates.Id, PersonId = persons.First(p => p.FullName == "Gore Verbinski").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = pirates.Id, PersonId = persons.First(p => p.FullName == "Johnny Depp").Id, Role = CreditRole.Actor, CharacterName = "Captain Jack Sparrow", DisplayOrder = 2 },
            new MovieCredit { MovieId = pirates.Id, PersonId = persons.First(p => p.FullName == "Orlando Bloom").Id, Role = CreditRole.Actor, CharacterName = "Will Turner", DisplayOrder = 3 },

            // Schindler's List
            new MovieCredit { MovieId = schindlerslist.Id, PersonId = persons.First(p => p.FullName == "Steven Spielberg").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = schindlerslist.Id, PersonId = persons.First(p => p.FullName == "Liam Neeson").Id, Role = CreditRole.Actor, CharacterName = "Oskar Schindler", DisplayOrder = 2 },
            new MovieCredit { MovieId = schindlerslist.Id, PersonId = persons.First(p => p.FullName == "Ralph Fiennes").Id, Role = CreditRole.Actor, CharacterName = "Amon Goeth", DisplayOrder = 3 },

            // The Good, the Bad and the Ugly
            new MovieCredit { MovieId = goodbadugly.Id, PersonId = persons.First(p => p.FullName == "Sergio Leone").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = goodbadugly.Id, PersonId = persons.First(p => p.FullName == "Clint Eastwood").Id, Role = CreditRole.Actor, CharacterName = "Blondie", DisplayOrder = 2 },
            new MovieCredit { MovieId = goodbadugly.Id, PersonId = persons.First(p => p.FullName == "Eli Wallach").Id, Role = CreditRole.Actor, CharacterName = "Tuco", DisplayOrder = 3 },

            // The Lord of the Rings: The Return of the King
            new MovieCredit { MovieId = lotr.Id, PersonId = persons.First(p => p.FullName == "Peter Jackson").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = lotr.Id, PersonId = persons.First(p => p.FullName == "Elijah Wood").Id, Role = CreditRole.Actor, CharacterName = "Frodo Baggins", DisplayOrder = 2 },
            new MovieCredit { MovieId = lotr.Id, PersonId = persons.First(p => p.FullName == "Viggo Mortensen").Id, Role = CreditRole.Actor, CharacterName = "Aragorn", DisplayOrder = 3 },

            // The Silence of the Lambs
            new MovieCredit { MovieId = silencelambs.Id, PersonId = persons.First(p => p.FullName == "Jonathan Demme").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = silencelambs.Id, PersonId = persons.First(p => p.FullName == "Anthony Hopkins").Id, Role = CreditRole.Actor, CharacterName = "Hannibal Lecter", DisplayOrder = 2 },
            new MovieCredit { MovieId = silencelambs.Id, PersonId = persons.First(p => p.FullName == "Jodie Foster").Id, Role = CreditRole.Actor, CharacterName = "Clarice Starling", DisplayOrder = 3 },

            // The Expendables
            new MovieCredit { MovieId = expendables.Id, PersonId = persons.First(p => p.FullName == "Sylvester Stallone").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = expendables.Id, PersonId = persons.First(p => p.FullName == "Sylvester Stallone").Id, Role = CreditRole.Actor, CharacterName = "Barney Ross", DisplayOrder = 2 },
            new MovieCredit { MovieId = expendables.Id, PersonId = persons.First(p => p.FullName == "Jason Statham").Id, Role = CreditRole.Actor, CharacterName = "Lee Christmas", DisplayOrder = 3 },

            // Indiana Jones
            new MovieCredit { MovieId = indianajones.Id, PersonId = persons.First(p => p.FullName == "Steven Spielberg").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = indianajones.Id, PersonId = persons.First(p => p.FullName == "Harrison Ford").Id, Role = CreditRole.Actor, CharacterName = "Indiana Jones", DisplayOrder = 2 },
            new MovieCredit { MovieId = indianajones.Id, PersonId = persons.First(p => p.FullName == "Karen Allen").Id, Role = CreditRole.Actor, CharacterName = "Marion Ravenwood", DisplayOrder = 3 },

            // Back to the Future
            new MovieCredit { MovieId = backtothefuture.Id, PersonId = persons.First(p => p.FullName == "Robert Zemeckis").Id, Role = CreditRole.Director, DisplayOrder = 1 },
            new MovieCredit { MovieId = backtothefuture.Id, PersonId = persons.First(p => p.FullName == "Michael J. Fox").Id, Role = CreditRole.Actor, CharacterName = "Marty McFly", DisplayOrder = 2 },
            new MovieCredit { MovieId = backtothefuture.Id, PersonId = persons.First(p => p.FullName == "Christopher Lloyd").Id, Role = CreditRole.Actor, CharacterName = "Doc Brown", DisplayOrder = 3 }

        };

        var existingMovieCreditKeys = await context.MovieCredits
            .Select(mc => $"{mc.MovieId}-{mc.PersonId}-{mc.Role}-{mc.DisplayOrder}")
            .ToListAsync();

        var missingMovieCredits = movieCredits
            .Where(mc => !existingMovieCreditKeys.Contains($"{mc.MovieId}-{mc.PersonId}-{mc.Role}-{mc.DisplayOrder}"))
            .ToList();

        if (missingMovieCredits.Count > 0)
        {
            context.MovieCredits.AddRange(missingMovieCredits);
            await context.SaveChangesAsync();
        }

        // =========================
        // Seed: Användare med recensioner
        // =========================
        var seedUsers = new List<(string Email, string FirstName, string LastName, string? Nickname, string Password)>
        {
            ("anna.lindstrom@retrovhs.se",   "Anna",    "Lindström",   "AnnaL",        "Seed123!"),
            ("erik.johansson@retrovhs.se",   "Erik",    "Johansson",   "ErikJ",        "Seed123!"),
            ("maria.karlsson@retrovhs.se",   "Maria",   "Karlsson",    "FilmMaria",    "Seed123!"),
            ("oscar.nilsson@retrovhs.se",    "Oscar",   "Nilsson",     "OscarN",       "Seed123!"),
            ("lisa.andersson@retrovhs.se",   "Lisa",    "Andersson",    null,           "Seed123!"),
            ("fredrik.berg@retrovhs.se",     "Fredrik",  "Berg",        "FredrikB",     "Seed123!"),
            ("sofia.ekstrom@retrovhs.se",    "Sofia",   "Ekström",     "SofiaE",       "Seed123!"),
            ("viktor.larsson@retrovhs.se",   "Viktor",  "Larsson",     "ViktorL",      "Seed123!"),
            ("emma.svensson@retrovhs.se",    "Emma",    "Svensson",    "EmmaS",        "Seed123!"),
            ("jakob.pettersson@retrovhs.se", "Jakob",   "Pettersson",  "JakobP",       "Seed123!"),
            ("hanna.olsson@retrovhs.se",     "Hanna",   "Olsson",      "HannaO",       "Seed123!"),
            ("adam.gustafsson@retrovhs.se",  "Adam",    "Gustafsson",  "AdamG",        "Seed123!"),
            ("klara.holm@retrovhs.se",       "Klara",   "Holm",         null,           "Seed123!"),
            ("nils.lundberg@retrovhs.se",    "Nils",    "Lundberg",    "NilsL",        "Seed123!"),
            ("elin.dahl@retrovhs.se",        "Elin",    "Dahl",        "ElinD",        "Seed123!"),
            ("simon.fransson@retrovhs.se",   "Simon",   "Fransson",    "SimonF",       "Seed123!"),
            ("amanda.hedlund@retrovhs.se",   "Amanda",  "Hedlund",     "AmandaH",      "Seed123!"),
            ("david.sjoberg@retrovhs.se",    "David",   "Sjöberg",     "DavidS",       "Seed123!"),
            ("maja.wikstrom@retrovhs.se",    "Maja",    "Wikström",    "MajaW",        "Seed123!"),
            ("lukas.blom@retrovhs.se",       "Lukas",   "Blom",        "LukasB",       "Seed123!"),
            ("saga.nord@retrovhs.se",        "Saga",    "Nord",         null,           "Seed123!"),
            ("oliver.lind@retrovhs.se",      "Oliver",  "Lind",        "OliverL",      "Seed123!"),
            ("wilma.ek@retrovhs.se",         "Wilma",   "Ek",          "WilmaE",       "Seed123!"),
            ("hugo.forsberg@retrovhs.se",    "Hugo",    "Forsberg",    "HugoF",        "Seed123!")
        };

        var createdUserIds = new Dictionary<string, int>();

        foreach (var (email, firstName, lastName, nickname, password) in seedUsers)
        {
            var existing = await userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existing != null)
            {
                createdUserIds[email] = existing.Id;
                continue;
            }

            var newUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Nickname = nickname,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(newUser, password);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, "Member");
                createdUserIds[email] = newUser.Id;
            }
        }

        // =========================
        // Seed: Reviews
        // =========================
        if (!await context.Reviews.AnyAsync())
        {
            var anna    = createdUserIds.GetValueOrDefault("anna.lindstrom@retrovhs.se");
            var erik    = createdUserIds.GetValueOrDefault("erik.johansson@retrovhs.se");
            var maria   = createdUserIds.GetValueOrDefault("maria.karlsson@retrovhs.se");
            var oscar   = createdUserIds.GetValueOrDefault("oscar.nilsson@retrovhs.se");
            var lisa    = createdUserIds.GetValueOrDefault("lisa.andersson@retrovhs.se");
            var fredrik = createdUserIds.GetValueOrDefault("fredrik.berg@retrovhs.se");
            var sofia   = createdUserIds.GetValueOrDefault("sofia.ekstrom@retrovhs.se");
            var viktor  = createdUserIds.GetValueOrDefault("viktor.larsson@retrovhs.se");
            var emma    = createdUserIds.GetValueOrDefault("emma.svensson@retrovhs.se");
            var jakob   = createdUserIds.GetValueOrDefault("jakob.pettersson@retrovhs.se");
            var hanna   = createdUserIds.GetValueOrDefault("hanna.olsson@retrovhs.se");
            var adam    = createdUserIds.GetValueOrDefault("adam.gustafsson@retrovhs.se");
            var klara   = createdUserIds.GetValueOrDefault("klara.holm@retrovhs.se");
            var nils    = createdUserIds.GetValueOrDefault("nils.lundberg@retrovhs.se");
            var elin    = createdUserIds.GetValueOrDefault("elin.dahl@retrovhs.se");
            var simon   = createdUserIds.GetValueOrDefault("simon.fransson@retrovhs.se");
            var amanda  = createdUserIds.GetValueOrDefault("amanda.hedlund@retrovhs.se");
            var david   = createdUserIds.GetValueOrDefault("david.sjoberg@retrovhs.se");
            var maja    = createdUserIds.GetValueOrDefault("maja.wikstrom@retrovhs.se");
            var lukas   = createdUserIds.GetValueOrDefault("lukas.blom@retrovhs.se");
            var saga    = createdUserIds.GetValueOrDefault("saga.nord@retrovhs.se");
            var oliver  = createdUserIds.GetValueOrDefault("oliver.lind@retrovhs.se");
            var wilma   = createdUserIds.GetValueOrDefault("wilma.ek@retrovhs.se");
            var hugo    = createdUserIds.GetValueOrDefault("hugo.forsberg@retrovhs.se");

            var seedReviews = new List<Review>
            {
                // The Godfather
                new Review { MovieId = godfather.Id, UserId = anna,    Rating = 5, Comment = "En av de bästa filmerna någonsin. Brando är fantastisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = godfather.Id, UserId = erik,    Rating = 5, Comment = "Mästerligt berättande och oförglömliga karaktärer.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },
                new Review { MovieId = godfather.Id, UserId = maria,   Rating = 4, Comment = "Klassiker. Lite lång men värd varje minut.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new Review { MovieId = godfather.Id, UserId = oscar,   Rating = 5, Comment = "Perfekt film. Inget att klaga på.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new Review { MovieId = godfather.Id, UserId = lisa,    Rating = 4, Comment = "Stark film med fantastiskt skådespeleri.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new Review { MovieId = godfather.Id, UserId = emma,    Rating = 5, Comment = "Brando och Pacino tillsammans – magiskt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-45) },
                new Review { MovieId = godfather.Id, UserId = jakob,   Rating = 4, Comment = "Tidlös maffiaklassiker. Tempot är perfekt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-42) },
                new Review { MovieId = godfather.Id, UserId = hanna,   Rating = 5, Comment = "Varje replik sitter. Filmen har allt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = godfather.Id, UserId = adam,    Rating = 5, Comment = "Pappas favoritfilm och nu min också.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = godfather.Id, UserId = nils,    Rating = 4, Comment = "Mäktig men kräver tålamod.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = godfather.Id, UserId = maja,    Rating = 5, Comment = "Bästa dramat genom tiderna, utan tvekan.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = godfather.Id, UserId = oliver,  Rating = 3, Comment = "Respekterar den men föredrar snabbare filmer.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Review { MovieId = godfather.Id, UserId = hugo,    Rating = 5, Comment = "Filmskapande på högsta möjliga nivå.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },

                // Pulp Fiction
                new Review { MovieId = pulp.Id, UserId = anna,    Rating = 5, Comment = "Tarantinos bästa! Dialogen är genial.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Review { MovieId = pulp.Id, UserId = erik,    Rating = 4, Comment = "Unik struktur och fantastisk soundtrack.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-22) },
                new Review { MovieId = pulp.Id, UserId = fredrik, Rating = 5, Comment = "Varje scen är ikonisk. Travolta och Jackson levererar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new Review { MovieId = pulp.Id, UserId = sofia,   Rating = 4, Comment = "Kul berättarstruktur, aldrig tråkig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new Review { MovieId = pulp.Id, UserId = emma,    Rating = 5, Comment = "Royale with Cheese – dialogen är oöverträffad.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-44) },
                new Review { MovieId = pulp.Id, UserId = hanna,   Rating = 4, Comment = "Stilig och smart men lite rörig ibland.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = pulp.Id, UserId = adam,    Rating = 5, Comment = "Tarantino skapade en helt ny filmstil.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = pulp.Id, UserId = simon,   Rating = 4, Comment = "Coolt men inte för alla. Jag älskar den.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-34) },
                new Review { MovieId = pulp.Id, UserId = david,   Rating = 3, Comment = "Snyggt gjord men för våldsam för min smak.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = pulp.Id, UserId = lukas,   Rating = 5, Comment = "Sett den 20+ gånger. Fortfarande fräsch.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = pulp.Id, UserId = wilma,   Rating = 4, Comment = "Uma Thurman-scenen på restaurangen – ikonisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },

                // The Shawshank Redemption
                new Review { MovieId = shawshank.Id, UserId = anna,    Rating = 5, Comment = "Hoppets film. Morgan Freeman är enastående.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = shawshank.Id, UserId = maria,   Rating = 5, Comment = "Gråter varje gång. Helt underbar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-21) },
                new Review { MovieId = shawshank.Id, UserId = oscar,   Rating = 5, Comment = "Den bästa dramafilmen som gjorts.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-16) },
                new Review { MovieId = shawshank.Id, UserId = viktor,  Rating = 4, Comment = "Gripande från start till slut.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-11) },
                new Review { MovieId = shawshank.Id, UserId = lisa,    Rating = 5, Comment = "Tidlös klassiker som alla borde se.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Review { MovieId = shawshank.Id, UserId = jakob,   Rating = 5, Comment = "Freeman och Robbins är otroliga tillsammans.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-43) },
                new Review { MovieId = shawshank.Id, UserId = klara,   Rating = 5, Comment = "Slutscenen ger mig frossa varje gång.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = shawshank.Id, UserId = elin,    Rating = 4, Comment = "Vacker film om hopp och vänskap.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = shawshank.Id, UserId = amanda,  Rating = 5, Comment = "Bästa filmen jag någonsin sett. Punkt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = shawshank.Id, UserId = saga,    Rating = 5, Comment = "Ren filmkonst. Brooks-scenen förstör mig.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = shawshank.Id, UserId = hugo,    Rating = 4, Comment = "Fantastisk men lite långsam i början.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },

                // The Dark Knight
                new Review { MovieId = darkknight.Id, UserId = erik,    Rating = 5, Comment = "Heath Ledgers Joker är oslagbar. Bästa superhjältefilmen.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },
                new Review { MovieId = darkknight.Id, UserId = oscar,   Rating = 5, Comment = "Mörk, intensiv och briljant.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-19) },
                new Review { MovieId = darkknight.Id, UserId = fredrik, Rating = 4, Comment = "Nolans bästa Batman-film utan tvekan.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Review { MovieId = darkknight.Id, UserId = viktor,  Rating = 5, Comment = "Mer thriller än superhjältefilm. Fantastiskt!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new Review { MovieId = darkknight.Id, UserId = jakob,   Rating = 5, Comment = "Ledger förtjänade varje pris han fick.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-41) },
                new Review { MovieId = darkknight.Id, UserId = adam,    Rating = 4, Comment = "Bästa actionfilmen 2000-talet, fight me.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = darkknight.Id, UserId = nils,    Rating = 5, Comment = "Why so serious? Gåshud varje gång.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = darkknight.Id, UserId = simon,   Rating = 4, Comment = "Inte bara en superhjältefilm – det är konst.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = darkknight.Id, UserId = maja,    Rating = 3, Comment = "Bra men överreklamerad tycker jag.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = darkknight.Id, UserId = oliver,  Rating = 5, Comment = "Definierade en hel genre på nytt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },

                // Fight Club
                new Review { MovieId = fightclub.Id, UserId = erik,    Rating = 5, Comment = "Mind-blowing twist. Brad Pitt är karismatisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },
                new Review { MovieId = fightclub.Id, UserId = oscar,   Rating = 4, Comment = "Provocerande och smart.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-17) },
                new Review { MovieId = fightclub.Id, UserId = viktor,  Rating = 5, Comment = "En film man måste se minst två gånger.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-13) },
                new Review { MovieId = fightclub.Id, UserId = emma,    Rating = 4, Comment = "Provocerande och annorlunda. Inte för alla.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-43) },
                new Review { MovieId = fightclub.Id, UserId = adam,    Rating = 5, Comment = "Första regeln: vi pratar inte om Fight Club.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = fightclub.Id, UserId = nils,    Rating = 4, Comment = "Smart samhällskritik insvept i våld.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = fightclub.Id, UserId = simon,   Rating = 5, Comment = "Fincher i toppform. Norton är underbar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = fightclub.Id, UserId = saga,    Rating = 3, Comment = "Inte min grej men förstår hypen.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = fightclub.Id, UserId = lukas,   Rating = 5, Comment = "Twisten slog mig som en smäll i ansiktet.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },

                // The Matrix
                new Review { MovieId = matrix.Id, UserId = anna,    Rating = 5, Comment = "Revolutionerade actiongenren. Ikonisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = matrix.Id, UserId = erik,    Rating = 4, Comment = "Fortfarande cool efter alla år.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-23) },
                new Review { MovieId = matrix.Id, UserId = sofia,   Rating = 5, Comment = "Keanu Reeves som Neo = perfekt casting.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new Review { MovieId = matrix.Id, UserId = fredrik, Rating = 4, Comment = "Filosofisk action på hög nivå.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new Review { MovieId = matrix.Id, UserId = hanna,   Rating = 5, Comment = "Bullet time förändrade filmhistorien.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-42) },
                new Review { MovieId = matrix.Id, UserId = adam,    Rating = 4, Comment = "Sci-fi-action som faktiskt får en att tänka.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = matrix.Id, UserId = klara,   Rating = 5, Comment = "Red pill eller blue pill? Genial film.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-34) },
                new Review { MovieId = matrix.Id, UserId = david,   Rating = 4, Comment = "Effekterna var banbrytande. Fortfarande bra.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = matrix.Id, UserId = lukas,   Rating = 3, Comment = "Bra idé men uppföljarna förstörde det.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Review { MovieId = matrix.Id, UserId = wilma,   Rating = 5, Comment = "Ikonisk film. Trinity är en badass.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },

                // Inception
                new Review { MovieId = inception.Id, UserId = anna,    Rating = 5, Comment = "Nolan levererar igen. Hjärnan smälter.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new Review { MovieId = inception.Id, UserId = maria,   Rating = 4, Comment = "Komplex men belönande. Snygga effekter.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Review { MovieId = inception.Id, UserId = viktor,  Rating = 5, Comment = "Slutet lämnar en tankfull i veckor.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Review { MovieId = inception.Id, UserId = emma,    Rating = 4, Comment = "Snurrade tollen nånsin? Snackar fortfarande om det.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-44) },
                new Review { MovieId = inception.Id, UserId = jakob,   Rating = 5, Comment = "Drömmar i drömmar i drömmar – briljant!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = inception.Id, UserId = nils,    Rating = 4, Comment = "Imponerande men krävde en andra titt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = inception.Id, UserId = elin,    Rating = 5, Comment = "Hans Zimmer-musiken gör filmen x10 bättre.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = inception.Id, UserId = amanda,  Rating = 3, Comment = "Smart men lite för invecklad.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = inception.Id, UserId = hugo,    Rating = 5, Comment = "Bästa sci-fi-filmen 2010-talet.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },

                // Scarface
                new Review { MovieId = scarface.Id, UserId = erik,    Rating = 5, Comment = "Say hello to my little friend! Ikonisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },
                new Review { MovieId = scarface.Id, UserId = oscar,   Rating = 4, Comment = "Al Pacino i toppform.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new Review { MovieId = scarface.Id, UserId = fredrik, Rating = 5, Comment = "Rå och oförglömlig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new Review { MovieId = scarface.Id, UserId = jakob,   Rating = 4, Comment = "Tony Montana är en av filmens bästa karaktärer.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-41) },
                new Review { MovieId = scarface.Id, UserId = adam,    Rating = 5, Comment = "Rå, brutal, ärlig. Miami i ett nötskal.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = scarface.Id, UserId = simon,   Rating = 3, Comment = "Lite för lång men Pacino räddar allt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-34) },
                new Review { MovieId = scarface.Id, UserId = david,   Rating = 4, Comment = "Gangsterfilm på steroider. Gillar det.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = scarface.Id, UserId = lukas,   Rating = 5, Comment = "The world is yours. Tidlöst.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Review { MovieId = scarface.Id, UserId = wilma,   Rating = 3, Comment = "Bra men väldigt maskulin energi.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },

                // Goodfellas
                new Review { MovieId = goodfellas.Id, UserId = anna,    Rating = 5, Comment = "Scorsese på topp. Tempot är perfekt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },
                new Review { MovieId = goodfellas.Id, UserId = oscar,   Rating = 5, Comment = "Liotta, De Niro, Pesci – drömlag.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-19) },
                new Review { MovieId = goodfellas.Id, UserId = lisa,    Rating = 4, Comment = "Gripping maffiafilm från start till slut.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-13) },
                new Review { MovieId = goodfellas.Id, UserId = hanna,   Rating = 5, Comment = "Funny how? Pesci-scenen är oslagbar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-43) },
                new Review { MovieId = goodfellas.Id, UserId = nils,    Rating = 4, Comment = "Bättre än Godfather – don't @ me.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = goodfellas.Id, UserId = elin,    Rating = 5, Comment = "Berättarrösten gör filmen så personlig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = goodfellas.Id, UserId = maja,    Rating = 4, Comment = "Snabb, energisk och full av liv.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = goodfellas.Id, UserId = oliver,  Rating = 3, Comment = "Bra men alla maffiafilmer börjar smälta ihop.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = goodfellas.Id, UserId = hugo,    Rating = 5, Comment = "Perfekt från första till sista scenen.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },

                // Interstellar
                new Review { MovieId = interstellar.Id, UserId = maria,   Rating = 5, Comment = "Gråter varje gång vid docking-scenen.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-22) },
                new Review { MovieId = interstellar.Id, UserId = sofia,   Rating = 5, Comment = "Visuellt mästerverk med hjärta.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-16) },
                new Review { MovieId = interstellar.Id, UserId = viktor,  Rating = 4, Comment = "Ambitiös sci-fi. Hans Zimmer-musiken!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new Review { MovieId = interstellar.Id, UserId = klara,   Rating = 5, Comment = "Murph! Tårar rinner. Fantastisk film.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-42) },
                new Review { MovieId = interstellar.Id, UserId = elin,    Rating = 4, Comment = "Vetenskapligt fascinerande och emotionellt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = interstellar.Id, UserId = simon,   Rating = 5, Comment = "Nolans mest personliga film. Älskar den.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = interstellar.Id, UserId = amanda,  Rating = 4, Comment = "Lång men värt varje minut.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = interstellar.Id, UserId = david,   Rating = 3, Comment = "Slutet var lite för abstrakt för mig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = interstellar.Id, UserId = saga,    Rating = 5, Comment = "Bästa filmupplevelsen i mitt liv.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-26) },

                // Gladiator
                new Review { MovieId = gladiator.Id, UserId = erik,    Rating = 5, Comment = "Are you not entertained?! Episk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },
                new Review { MovieId = gladiator.Id, UserId = fredrik, Rating = 4, Comment = "Russell Crowe blev född för denna roll.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-17) },
                new Review { MovieId = gladiator.Id, UserId = lisa,    Rating = 5, Comment = "Kraftfull och emotionell. Älskar den.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-11) },
                new Review { MovieId = gladiator.Id, UserId = jakob,   Rating = 5, Comment = "Maximus Decimus Meridius – vilken karaktär!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = gladiator.Id, UserId = hanna,   Rating = 4, Comment = "Vacker film trots allt våld.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = gladiator.Id, UserId = nils,    Rating = 5, Comment = "Episk i alla bemärkelser. Musiken är fantastisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-34) },
                new Review { MovieId = gladiator.Id, UserId = simon,   Rating = 4, Comment = "Joaquin Phoenix som skurk = magi.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = gladiator.Id, UserId = maja,    Rating = 3, Comment = "Lite förutsägbar men bra gjord.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Review { MovieId = gladiator.Id, UserId = wilma,   Rating = 5, Comment = "Gråter vid slutet. Varje. Gång.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },

                // Forrest Gump
                new Review { MovieId = forrestgump.Id, UserId = anna,    Rating = 5, Comment = "Life is like a box of chocolates. Underbar film.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-23) },
                new Review { MovieId = forrestgump.Id, UserId = maria,   Rating = 5, Comment = "Tom Hanks bästa roll. Varm och rolig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new Review { MovieId = forrestgump.Id, UserId = sofia,   Rating = 4, Comment = "Charmig och rörande berättelse.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new Review { MovieId = forrestgump.Id, UserId = emma,    Rating = 5, Comment = "En film som gör en glad och ledsen samtidigt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-44) },
                new Review { MovieId = forrestgump.Id, UserId = klara,   Rating = 4, Comment = "Hanks är genial. Historien är fin men lite naiv.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = forrestgump.Id, UserId = elin,    Rating = 5, Comment = "Run Forrest run! Tidlös och varm.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = forrestgump.Id, UserId = amanda,  Rating = 5, Comment = "Gråter som en bäck. Underbar film.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = forrestgump.Id, UserId = saga,    Rating = 4, Comment = "Söt film men lite för sockrig emellanåt.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = forrestgump.Id, UserId = oliver,  Rating = 3, Comment = "Nostalgisk men inte min favorit.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },

                // Titanic
                new Review { MovieId = titanic.Id, UserId = maria,   Rating = 5, Comment = "Romantik och tragedi i perfekt balans.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = titanic.Id, UserId = sofia,   Rating = 4, Comment = "Visuellt imponerande och gripande.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new Review { MovieId = titanic.Id, UserId = lisa,    Rating = 5, Comment = "Jack kunde ha fått plats på dörren! Men fantastisk film.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Review { MovieId = titanic.Id, UserId = emma,    Rating = 5, Comment = "DiCaprio och Winslet = filmkemi av guld.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-43) },
                new Review { MovieId = titanic.Id, UserId = hanna,   Rating = 4, Comment = "Romantiskt och tragiskt. Camerons mästerverk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = titanic.Id, UserId = klara,   Rating = 5, Comment = "Gråter VARJE gång. Utan undantag.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = titanic.Id, UserId = elin,    Rating = 4, Comment = "Vacker kärlekshistoria mot en fruktansvärd bakgrund.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = titanic.Id, UserId = maja,    Rating = 3, Comment = "Lite för lång och melodramatisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = titanic.Id, UserId = wilma,   Rating = 5, Comment = "Near, far, wherever you are... ikonisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = titanic.Id, UserId = hugo,    Rating = 3, Comment = "Bra film men överreklamerad.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },

                // Terminator 2
                new Review { MovieId = terminator2.Id, UserId = erik,    Rating = 5, Comment = "Bästa uppföljaren någonsin. Hasta la vista, baby!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-21) },
                new Review { MovieId = terminator2.Id, UserId = oscar,   Rating = 5, Comment = "Arnold i sin ikoniska roll. Perfekt action.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new Review { MovieId = terminator2.Id, UserId = viktor,  Rating = 4, Comment = "Effekterna håller fortfarande.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new Review { MovieId = terminator2.Id, UserId = adam,    Rating = 5, Comment = "T-1000 i flytande metall var banbrytande.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-42) },
                new Review { MovieId = terminator2.Id, UserId = nils,    Rating = 5, Comment = "Cameron + Schwarzenegger = guld.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = terminator2.Id, UserId = simon,   Rating = 4, Comment = "Bättre än ettan på alla sätt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = terminator2.Id, UserId = david,   Rating = 4, Comment = "Actionfilm som faktiskt har hjärta.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = terminator2.Id, UserId = lukas,   Rating = 5, Comment = "I'll be back. Och jag tittar om den varje år.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Review { MovieId = terminator2.Id, UserId = oliver,  Rating = 3, Comment = "Bra action men lite daterad nu.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },

                // Die Hard
                new Review { MovieId = diehard.Id, UserId = fredrik, Rating = 5, Comment = "Den bästa julfilmen! Yippee-ki-yay.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-22) },
                new Review { MovieId = diehard.Id, UserId = oscar,   Rating = 4, Comment = "Bruce Willis definierade actionhjälten.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-16) },
                new Review { MovieId = diehard.Id, UserId = jakob,   Rating = 5, Comment = "ÄR det en julfilm? JA. Och den bästa.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-41) },
                new Review { MovieId = diehard.Id, UserId = adam,    Rating = 4, Comment = "Willis mot Rickman – perfekt matchup.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = diehard.Id, UserId = nils,    Rating = 5, Comment = "Kan inte fira jul utan att se Die Hard.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-34) },
                new Review { MovieId = diehard.Id, UserId = simon,   Rating = 4, Comment = "Alan Rickman som Hans Gruber = filmhistoria.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = diehard.Id, UserId = david,   Rating = 3, Comment = "Underhållande men inte så smart som folk tror.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Review { MovieId = diehard.Id, UserId = lukas,   Rating = 5, Comment = "Bästa actionfilmen genom tiderna.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },
                new Review { MovieId = diehard.Id, UserId = hugo,    Rating = 4, Comment = "Klassiker som aldrig tröttnar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-22) },

                // Alien
                new Review { MovieId = alien.Id, UserId = anna,    Rating = 5, Comment = "Skrämmande på riktigt. Sigourney Weaver är bäst.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-19) },
                new Review { MovieId = alien.Id, UserId = sofia,   Rating = 4, Comment = "Stämningen i det här filmens är oöverträffad.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-13) },
                new Review { MovieId = alien.Id, UserId = emma,    Rating = 5, Comment = "In space no one can hear you scream. Genialiskt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-42) },
                new Review { MovieId = alien.Id, UserId = hanna,   Rating = 4, Comment = "Obehaglig och perfekt. Ripley är ikonisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = alien.Id, UserId = klara,   Rating = 3, Comment = "Bra stämning men lite långsam.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = alien.Id, UserId = nils,    Rating = 5, Comment = "Sci-fi-skräck när den är som bäst.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = alien.Id, UserId = amanda,  Rating = 4, Comment = "Chestburster-scenen ger mig fortfarande mardrömmar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Review { MovieId = alien.Id, UserId = maja,    Rating = 5, Comment = "Ridley Scott skapade sci-fi-skräcken.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },

                // The Shining
                new Review { MovieId = shining.Id, UserId = anna,    Rating = 5, Comment = "Here's Johnny! Kubrick på sin bäst.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new Review { MovieId = shining.Id, UserId = maria,   Rating = 4, Comment = "Läskig utan att förlita sig på jump scares.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new Review { MovieId = shining.Id, UserId = viktor,  Rating = 5, Comment = "Jack Nicholson förvandlas helt. Mästerligt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new Review { MovieId = shining.Id, UserId = emma,    Rating = 4, Comment = "Stämningen i Overlook Hotel är magisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-43) },
                new Review { MovieId = shining.Id, UserId = klara,   Rating = 5, Comment = "Kubrick skapade ett skräckmästerverk.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = shining.Id, UserId = elin,    Rating = 4, Comment = "Tvillingarna i korridoren = rena mardrömsscenen.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = shining.Id, UserId = simon,   Rating = 5, Comment = "Bästa skräckfilmen som gjorts. Punkt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = shining.Id, UserId = saga,    Rating = 3, Comment = "Inte lika läskig som jag hoppats.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = shining.Id, UserId = hugo,    Rating = 5, Comment = "Nicholson är besatt. Fantastisk prestation.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },

                // Se7en
                new Review { MovieId = se7en.Id, UserId = erik,    Rating = 5, Comment = "Mörk, brutal och briljant. Slutet är chockerande.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new Review { MovieId = se7en.Id, UserId = lisa,    Rating = 4, Comment = "Fincher skapar en otrolig atmosfär.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Review { MovieId = se7en.Id, UserId = jakob,   Rating = 5, Comment = "What's in the box?! Slutet förstörde mig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-41) },
                new Review { MovieId = se7en.Id, UserId = adam,    Rating = 4, Comment = "Mörk thriller som håller hela vägen.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = se7en.Id, UserId = nils,    Rating = 5, Comment = "Pitt och Freeman är ett fantastiskt duo.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-34) },
                new Review { MovieId = se7en.Id, UserId = elin,    Rating = 3, Comment = "Bra men lite för mörk för mig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = se7en.Id, UserId = david,   Rating = 4, Comment = "Regnet och mörkret sätter perfekt stämning.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = se7en.Id, UserId = wilma,   Rating = 5, Comment = "Bästa detektivthrillern. Ingen tvekan.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },

                // Avatar
                new Review { MovieId = avatar.Id, UserId = maria,   Rating = 4, Comment = "Visuellt spektakulär. Handlingen är enklare men fungerar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },
                new Review { MovieId = avatar.Id, UserId = sofia,   Rating = 4, Comment = "Pandora är otroligt vacker. Bra i 3D.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new Review { MovieId = avatar.Id, UserId = viktor,  Rating = 3, Comment = "Snygg men handlingen är förutsägbar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-11) },
                new Review { MovieId = avatar.Id, UserId = emma,    Rating = 4, Comment = "Camerons visuella spektakel. Handlingen är enkel.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-42) },
                new Review { MovieId = avatar.Id, UserId = klara,   Rating = 3, Comment = "Vacker men glömbar handling.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = avatar.Id, UserId = simon,   Rating = 2, Comment = "Pocahontas i rymden. Inget nytt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = avatar.Id, UserId = amanda,  Rating = 4, Comment = "Fantastisk i bio. Hemma på TV? Inte lika bra.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = avatar.Id, UserId = lukas,   Rating = 3, Comment = "Effekterna håller men storyn gör det inte.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Review { MovieId = avatar.Id, UserId = oliver,  Rating = 2, Comment = "Överskattad. Snyggt men tomt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },

                // Blood In Blood Out
                new Review { MovieId = blood.Id, UserId = erik,    Rating = 5, Comment = "Underskattad klassiker. Rå och äkta.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },
                new Review { MovieId = blood.Id, UserId = oscar,   Rating = 4, Comment = "Tre timmar men aldrig tråkigt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-17) },
                new Review { MovieId = blood.Id, UserId = jakob,   Rating = 5, Comment = "En av de mest underskattade filmerna. Riktigt stark.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = blood.Id, UserId = adam,    Rating = 4, Comment = "Brutal verklighet. Tre olika vägar i livet.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = blood.Id, UserId = nils,    Rating = 3, Comment = "Lång och tung men bra skådespeleri.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = blood.Id, UserId = simon,   Rating = 5, Comment = "Vato loco forever! Kultfilm.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = blood.Id, UserId = david,   Rating = 4, Comment = "Äkta och gripande. Rekommenderar starkt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = blood.Id, UserId = lukas,   Rating = 3, Comment = "Bra men förtjänar en bättre produktion.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },

                // Jurassic Park
                new Review { MovieId = jurassicpark.Id, UserId = anna,    Rating = 5, Comment = "Spielberg-magi. Dinosaurierna känns äkta!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-21) },
                new Review { MovieId = jurassicpark.Id, UserId = sofia,   Rating = 4, Comment = "Nostalgitrip varje gång. Musiken!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Review { MovieId = jurassicpark.Id, UserId = emma,    Rating = 5, Comment = "Dinosaurier + Spielberg = drömkombo.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-43) },
                new Review { MovieId = jurassicpark.Id, UserId = hanna,   Rating = 4, Comment = "T-Rex-scenen med vattenglaset – perfekt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = jurassicpark.Id, UserId = klara,   Rating = 5, Comment = "John Williams-musiken ger frossa.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = jurassicpark.Id, UserId = elin,    Rating = 4, Comment = "Effekterna från -93 ser bättre ut än moderna CGI.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = jurassicpark.Id, UserId = amanda,  Rating = 3, Comment = "Kul men barnslig. Nostalgin gör mycket.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = jurassicpark.Id, UserId = oliver,  Rating = 5, Comment = "Life finds a way. Älskar denna film.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },
                new Review { MovieId = jurassicpark.Id, UserId = hugo,    Rating = 4, Comment = "Jeff Goldblum gör allting bättre.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-23) },

                // Heat
                new Review { MovieId = heat.Id, UserId = oscar,   Rating = 5, Comment = "De Niro och Pacino äntligen face to face. Episkt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-19) },
                new Review { MovieId = heat.Id, UserId = fredrik, Rating = 5, Comment = "Bankrånsscenen är filmhistoriens bästa.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-13) },
                new Review { MovieId = heat.Id, UserId = adam,    Rating = 5, Comment = "Två ikoner i samma scen. Magiskt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = heat.Id, UserId = nils,    Rating = 4, Comment = "Lång men varje minut är värd det.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = heat.Id, UserId = simon,   Rating = 5, Comment = "Manns bästa film. Shootout-scenen!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = heat.Id, UserId = david,   Rating = 4, Comment = "Stilren och intelligent actionfilm.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = heat.Id, UserId = lukas,   Rating = 3, Comment = "Bra men lite för lång för min smak.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = heat.Id, UserId = maja,    Rating = 4, Comment = "Val Kilmer stjäl scener. Underbar film.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },
                new Review { MovieId = heat.Id, UserId = hugo,    Rating = 5, Comment = "Perfekt krimfilm. Inget fat.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-21) },

                // Rocky
                new Review { MovieId = rocky.Id, UserId = fredrik, Rating = 5, Comment = "Stallone skrev och spelade en odödlig karaktär.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new Review { MovieId = rocky.Id, UserId = viktor,  Rating = 4, Comment = "Underdog-historien som aldrig blir gammal.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Review { MovieId = rocky.Id, UserId = jakob,   Rating = 5, Comment = "Adrian! Tårar varje gång.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-41) },
                new Review { MovieId = rocky.Id, UserId = adam,    Rating = 4, Comment = "Inte bara en boxningsfilm – det är en kärlekshistoria.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = rocky.Id, UserId = nils,    Rating = 5, Comment = "Gonna fly now! Musiken är legendarisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-34) },
                new Review { MovieId = rocky.Id, UserId = simon,   Rating = 3, Comment = "Bra men lite cheesy.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = rocky.Id, UserId = david,   Rating = 4, Comment = "Stallone förtjänade en Oscar för den här.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = rocky.Id, UserId = lukas,   Rating = 5, Comment = "Trapporna i Philadelphia – ikoniskt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },
                new Review { MovieId = rocky.Id, UserId = hugo,    Rating = 4, Comment = "Inspirerande på riktigt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-21) },

                // Saving Private Ryan
                new Review { MovieId = savingprivateryan.Id, UserId = anna,    Rating = 5, Comment = "Omaha Beach-scenen är brutal och oförglömlig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-17) },
                new Review { MovieId = savingprivateryan.Id, UserId = oscar,   Rating = 5, Comment = "Bästa krigsfilmen. Punkt.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-11) },
                new Review { MovieId = savingprivateryan.Id, UserId = emma,    Rating = 5, Comment = "Öppningsscenen förändrade krigsfilmsgenren.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-43) },
                new Review { MovieId = savingprivateryan.Id, UserId = hanna,   Rating = 4, Comment = "Fruktansvärt realistisk. Tom Hanks bästa.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = savingprivateryan.Id, UserId = klara,   Rating = 5, Comment = "Earn this. Slutet knäcker mig.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = savingprivateryan.Id, UserId = nils,    Rating = 4, Comment = "Spielberg på topp. Svårt att se men nödvändigt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = savingprivateryan.Id, UserId = david,   Rating = 5, Comment = "Bästa krigsfilmen jag sett. Grym.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = savingprivateryan.Id, UserId = saga,    Rating = 4, Comment = "Tung men viktig film.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-26) },
                new Review { MovieId = savingprivateryan.Id, UserId = wilma,   Rating = 5, Comment = "Gråter från första till sista scenen.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-23) },

                // No Country for Old Men
                new Review { MovieId = nocountry.Id, UserId = erik,    Rating = 5, Comment = "Javier Bardem som Chigurh = ren skräck.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-16) },
                new Review { MovieId = nocountry.Id, UserId = lisa,    Rating = 4, Comment = "Coen-bröderna levererar som vanligt.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new Review { MovieId = nocountry.Id, UserId = adam,    Rating = 5, Comment = "Bardems frisyr är lika skrämmande som karaktären.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = nocountry.Id, UserId = nils,    Rating = 4, Comment = "Coen-bröderna skapar något unikt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = nocountry.Id, UserId = elin,    Rating = 5, Comment = "Spännande och otäck. Bultmejseln!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = nocountry.Id, UserId = maja,    Rating = 3, Comment = "Bra men slutet var förvirrande.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = nocountry.Id, UserId = oliver,  Rating = 4, Comment = "Mörk och intelligent. Inte för alla.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = nocountry.Id, UserId = hugo,    Rating = 5, Comment = "Coen-brödernas absoluta mästerverk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },

                // The Silence of the Lambs
                new Review { MovieId = silencelambs.Id, UserId = anna,    Rating = 5, Comment = "Hopkins är skrämmande bra som Hannibal.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new Review { MovieId = silencelambs.Id, UserId = maria,   Rating = 5, Comment = "Psykologisk thriller på allra högsta nivå.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new Review { MovieId = silencelambs.Id, UserId = emma,    Rating = 5, Comment = "Hopkins och Foster – vilken kombination!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-42) },
                new Review { MovieId = silencelambs.Id, UserId = hanna,   Rating = 4, Comment = "Obehaglig men briljant. Clarice är en ikon.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = silencelambs.Id, UserId = klara,   Rating = 5, Comment = "Fava beans and a nice Chianti. Gåshud.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = silencelambs.Id, UserId = elin,    Rating = 4, Comment = "Psykologisk thriller som sätter standarden.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = silencelambs.Id, UserId = maja,    Rating = 3, Comment = "Bra men inte så läskig som jag förväntade.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Review { MovieId = silencelambs.Id, UserId = saga,    Rating = 5, Comment = "Bästa thrillern genom tiderna.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-25) },
                new Review { MovieId = silencelambs.Id, UserId = wilma,   Rating = 5, Comment = "Jodie Foster bär filmen med känsla.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-22) },

                // Braveheart – ska ha 5.0 i average!
                new Review { MovieId = braveheart.Id, UserId = erik,    Rating = 5, Comment = "FREEDOM! Mel Gibson i sitt livs roll.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Review { MovieId = braveheart.Id, UserId = fredrik, Rating = 5, Comment = "Episkt slagfält och stark berättelse. Perfekt film.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Review { MovieId = braveheart.Id, UserId = anna,    Rating = 5, Comment = "Gråter varje gång. William Wallace är en legend.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new Review { MovieId = braveheart.Id, UserId = oscar,   Rating = 5, Comment = "Bästa krigsfilmen genom tiderna. Episk på alla sätt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new Review { MovieId = braveheart.Id, UserId = maria,   Rating = 5, Comment = "Musiken, skådespeleriet, allt är perfekt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new Review { MovieId = braveheart.Id, UserId = lisa,    Rating = 5, Comment = "En film som berör på djupet. Mästerligt.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-6) },
                new Review { MovieId = braveheart.Id, UserId = emma,    Rating = 5, Comment = "FREEDOM! Gåshud från start till slut.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-44) },
                new Review { MovieId = braveheart.Id, UserId = jakob,   Rating = 5, Comment = "Episkt mästerverk. Gibson regisserar och spelar perfekt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-41) },
                new Review { MovieId = braveheart.Id, UserId = hanna,   Rating = 5, Comment = "Gråter varje gång vid slutet. Ren filmkonst.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = braveheart.Id, UserId = adam,    Rating = 5, Comment = "William Wallace är filmens bästa hjälte.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = braveheart.Id, UserId = klara,   Rating = 5, Comment = "Kraftfull, vacker och hjärtskärande.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = braveheart.Id, UserId = nils,    Rating = 5, Comment = "Slagfältsscenerna är de bästa i filmhistorien.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = braveheart.Id, UserId = elin,    Rating = 5, Comment = "Musiken av James Horner förstärker allt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },
                new Review { MovieId = braveheart.Id, UserId = simon,   Rating = 5, Comment = "Inget annat ord än mästerverk passar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-23) },
                new Review { MovieId = braveheart.Id, UserId = amanda,  Rating = 5, Comment = "Ser den varje år. Blir aldrig sämre.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new Review { MovieId = braveheart.Id, UserId = david,   Rating = 5, Comment = "Filmens mest inspirerande tal. FREEDOM!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-17) },
                new Review { MovieId = braveheart.Id, UserId = maja,    Rating = 5, Comment = "Allt stämmer. Felfri film.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Review { MovieId = braveheart.Id, UserId = lukas,   Rating = 5, Comment = "Den enda filmen som förtjänar 5/5 från alla.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-11) },
                new Review { MovieId = braveheart.Id, UserId = saga,    Rating = 5, Comment = "Historiskt drama när det är som allra bäst.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Review { MovieId = braveheart.Id, UserId = oliver,  Rating = 5, Comment = "Tre timmar av ren filmperfektion.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Review { MovieId = braveheart.Id, UserId = wilma,   Rating = 5, Comment = "Hjältemod, kärlek och uppoffring. Perfekt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-3) },
                new Review { MovieId = braveheart.Id, UserId = hugo,    Rating = 5, Comment = "Ingen film har berört mig mer. 5/5 utan tvekan.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-1) },

                // Back to the Future
                new Review { MovieId = backtothefuture.Id, UserId = sofia,   Rating = 5, Comment = "Tidlös (pun intended). Perfekt äventyrsfilm.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-16) },
                new Review { MovieId = backtothefuture.Id, UserId = viktor,  Rating = 5, Comment = "Michael J. Fox och Christopher Lloyd = guld.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new Review { MovieId = backtothefuture.Id, UserId = emma,    Rating = 5, Comment = "DeLorean-bilen är filmens coolaste prop.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-43) },
                new Review { MovieId = backtothefuture.Id, UserId = klara,   Rating = 4, Comment = "Rolig och smart. Perfekt familjefilm.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = backtothefuture.Id, UserId = nils,    Rating = 5, Comment = "1.21 gigawatts! Klassiker i ordets sanna mening.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = backtothefuture.Id, UserId = amanda,  Rating = 4, Comment = "Charm och humor i en perfekt mix.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = backtothefuture.Id, UserId = david,   Rating = 5, Comment = "Bästa tidsresefilmen. Ingen tvekan.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = backtothefuture.Id, UserId = maja,    Rating = 4, Comment = "Fortfarande kul efter alla år.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },
                new Review { MovieId = backtothefuture.Id, UserId = lukas,   Rating = 5, Comment = "Zemeckis levererade magi.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-23) },
                new Review { MovieId = backtothefuture.Id, UserId = wilma,   Rating = 3, Comment = "Rolig men lite barnförpackad.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-20) },

                // LOTR
                new Review { MovieId = lotr.Id, UserId = maria,   Rating = 5, Comment = "Det perfekta avslutet. Gråter vid 'You bow to no one'.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new Review { MovieId = lotr.Id, UserId = anna,    Rating = 5, Comment = "Peter Jackson skapade magi. Oöverträffat.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new Review { MovieId = lotr.Id, UserId = oscar,   Rating = 5, Comment = "Episkt i ordets sanna bemärkelse.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new Review { MovieId = lotr.Id, UserId = emma,    Rating = 5, Comment = "Det bästa avslutet i filmhistorien.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-44) },
                new Review { MovieId = lotr.Id, UserId = jakob,   Rating = 5, Comment = "For Frodo! Gåshud varje gång.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = lotr.Id, UserId = hanna,   Rating = 4, Comment = "Episkt men många falska slut.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = lotr.Id, UserId = klara,   Rating = 5, Comment = "Tolkien vore stolt. Jackson levererade.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = lotr.Id, UserId = elin,    Rating = 5, Comment = "Trilogin förtjänar varje Oscar den fick.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = lotr.Id, UserId = amanda,  Rating = 4, Comment = "Fantastiskt men låååång.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = lotr.Id, UserId = oliver,  Rating = 5, Comment = "Filmhistoriens bästa trilogi.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },
                new Review { MovieId = lotr.Id, UserId = wilma,   Rating = 5, Comment = "Rider of Rohan-scenen = bästa scenen.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-21) },
                new Review { MovieId = lotr.Id, UserId = hugo,    Rating = 5, Comment = "Magi, vänskap och mod. Perfekt film.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-18) },

                // The Exorcist
                new Review { MovieId = exorcist.Id, UserId = lisa,    Rating = 4, Comment = "Fortfarande skrämmande efter 50 år.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-13) },
                new Review { MovieId = exorcist.Id, UserId = viktor,  Rating = 4, Comment = "Klassisk skräck som sätter standarden.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new Review { MovieId = exorcist.Id, UserId = jakob,   Rating = 3, Comment = "Lite daterad men fortfarande obehaglig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = exorcist.Id, UserId = adam,    Rating = 4, Comment = "Skräck som bygger på ångest, inte jump scares.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = exorcist.Id, UserId = nils,    Rating = 5, Comment = "Bästa skräckfilmen som gjorts. Ingen diskussion.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = exorcist.Id, UserId = elin,    Rating = 2, Comment = "För gammal och långsam för min smak.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = exorcist.Id, UserId = simon,   Rating = 4, Comment = "Stämningen är fortfarande fantastisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = exorcist.Id, UserId = saga,    Rating = 3, Comment = "Respekterar den men den skrämmer inte mig.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-24) },

                // Casino
                new Review { MovieId = casino.Id, UserId = oscar,   Rating = 4, Comment = "Scorsese, De Niro, Pesci – funkar varje gång.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new Review { MovieId = casino.Id, UserId = erik,    Rating = 4, Comment = "Lång men underhållande från start till slut.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new Review { MovieId = casino.Id, UserId = adam,    Rating = 5, Comment = "Las Vegas-glamour och gangsterbrutalitet. Perfekt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = casino.Id, UserId = nils,    Rating = 3, Comment = "Bra men Goodfellas är bättre.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = casino.Id, UserId = simon,   Rating = 4, Comment = "Sharon Stone stjäl filmen.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = casino.Id, UserId = david,   Rating = 4, Comment = "Scorseses mest visuellt stiliga film.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = casino.Id, UserId = lukas,   Rating = 3, Comment = "Lite för lång men bra skådespeleri.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },
                new Review { MovieId = casino.Id, UserId = maja,    Rating = 4, Comment = "Pesci är lika bra här som i Goodfellas.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-23) },

                // American History X
                new Review { MovieId = americanhistoryx.Id, UserId = anna,    Rating = 5, Comment = "Edward Norton levererar en karriärbästa prestation.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Review { MovieId = americanhistoryx.Id, UserId = lisa,    Rating = 4, Comment = "Stark och viktig film.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Review { MovieId = americanhistoryx.Id, UserId = emma,    Rating = 5, Comment = "Norton borde ha vunnit Oscar. Fantastisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-42) },
                new Review { MovieId = americanhistoryx.Id, UserId = hanna,   Rating = 4, Comment = "Viktig film som alla borde se.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = americanhistoryx.Id, UserId = klara,   Rating = 5, Comment = "Mäktig och smärtsam. Berör på djupet.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = americanhistoryx.Id, UserId = nils,    Rating = 4, Comment = "Rå och ärlig. Svår att titta på men nödvändig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = americanhistoryx.Id, UserId = maja,    Rating = 3, Comment = "Bra men slutet var abrupt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Review { MovieId = americanhistoryx.Id, UserId = wilma,   Rating = 5, Comment = "En av de mest kraftfulla filmerna jag sett.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },
                new Review { MovieId = americanhistoryx.Id, UserId = hugo,    Rating = 4, Comment = "Nortons transformation är otrolig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-22) },

                // Taxi Driver
                new Review { MovieId = taxidriver.Id, UserId = erik,    Rating = 5, Comment = "You talkin' to me? De Niro definierar en genre.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new Review { MovieId = taxidriver.Id, UserId = oscar,   Rating = 4, Comment = "Mörkt porträtt av ensamhet i storstaden.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new Review { MovieId = taxidriver.Id, UserId = jakob,   Rating = 5, Comment = "Scorsese och De Niro – drömteam.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-41) },
                new Review { MovieId = taxidriver.Id, UserId = adam,    Rating = 4, Comment = "New York har aldrig sett mörkare ut.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = taxidriver.Id, UserId = nils,    Rating = 5, Comment = "En film som aldrig lämnar en.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-34) },
                new Review { MovieId = taxidriver.Id, UserId = elin,    Rating = 3, Comment = "Bra skådespeleri men för dyster.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = taxidriver.Id, UserId = simon,   Rating = 4, Comment = "De Niro som Travis Bickle = filmhistoria.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = taxidriver.Id, UserId = oliver,  Rating = 5, Comment = "Ensamhetens film. Briljant och mörk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },

                // Predator
                new Review { MovieId = predator.Id, UserId = erik,    Rating = 5, Comment = "Get to the choppa! Arnold i djungeln = action-perfektion.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-22) },
                new Review { MovieId = predator.Id, UserId = oscar,   Rating = 4, Comment = "Klassisk 80-tals action med sci-fi-twist.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-16) },
                new Review { MovieId = predator.Id, UserId = fredrik, Rating = 4, Comment = "Svettigt, spännande och brutalt. Älskar den.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new Review { MovieId = predator.Id, UserId = adam,    Rating = 5, Comment = "Arnolds biceps och en alien – vad mer behövs?", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = predator.Id, UserId = nils,    Rating = 4, Comment = "Perfekt mix av action och sci-fi.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = predator.Id, UserId = simon,   Rating = 4, Comment = "If it bleeds, we can kill it. Ikoniskt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = predator.Id, UserId = david,   Rating = 3, Comment = "Bra action men ganska enkel handling.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = predator.Id, UserId = lukas,   Rating = 5, Comment = "80-tals action i dess bästa form.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = predator.Id, UserId = hugo,    Rating = 4, Comment = "Predatorn är en av filmhistoriens bästa monster.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },

                // A Nightmare on Elm Street
                new Review { MovieId = nightmare.Id, UserId = anna,    Rating = 4, Comment = "Freddy Krueger är ikonisk. Skrämmande koncept.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-21) },
                new Review { MovieId = nightmare.Id, UserId = lisa,    Rating = 3, Comment = "Lite dated men fortfarande obehaglig.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new Review { MovieId = nightmare.Id, UserId = viktor,  Rating = 4, Comment = "Originell skräck. Wes Craven var ett geni.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new Review { MovieId = nightmare.Id, UserId = emma,    Rating = 3, Comment = "Konceptet är briljant men filmen har åldrats.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-42) },
                new Review { MovieId = nightmare.Id, UserId = hanna,   Rating = 4, Comment = "Freddy är skräckens clown. Obehaglig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = nightmare.Id, UserId = elin,    Rating = 2, Comment = "Inte lika läskig som jag hoppats.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = nightmare.Id, UserId = simon,   Rating = 4, Comment = "Wes Craven skapade en ikonisk karaktär.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = nightmare.Id, UserId = saga,    Rating = 3, Comment = "OK men inte bland de bästa skräckfilmerna.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-28) },

                // Friday the 13th
                new Review { MovieId = friday13.Id, UserId = oscar,   Rating = 3, Comment = "Klassiker men handlingen är ganska tunn.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new Review { MovieId = friday13.Id, UserId = erik,    Rating = 3, Comment = "Nostalgisk men inte lika bra som jag mindes.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Review { MovieId = friday13.Id, UserId = sofia,   Rating = 2, Comment = "För simpel för min smak. Skräcken funkar inte riktigt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Review { MovieId = friday13.Id, UserId = adam,    Rating = 3, Comment = "Jason är cool men filmen är sådär.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = friday13.Id, UserId = nils,    Rating = 2, Comment = "Slasher-mall. Förutsägbar från minut ett.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = friday13.Id, UserId = elin,    Rating = 3, Comment = "Historisk betydelse men inte så bra.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = friday13.Id, UserId = maja,    Rating = 1, Comment = "Tråkig och ointressant. Sorry.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = friday13.Id, UserId = oliver,  Rating = 3, Comment = "Nostalgi bär den men inte mycket mer.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },
                new Review { MovieId = friday13.Id, UserId = hugo,    Rating = 2, Comment = "Sämre än jag förväntade mig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-23) },

                // Halloween
                new Review { MovieId = halloween.Id, UserId = anna,    Rating = 5, Comment = "Michael Myers är skräckens konung. Perfekt tempo.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-19) },
                new Review { MovieId = halloween.Id, UserId = maria,   Rating = 4, Comment = "Carpenters musik gör halva jobbet. Briljant.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-13) },
                new Review { MovieId = halloween.Id, UserId = viktor,  Rating = 4, Comment = "Simpelt men effektivt. Jamie Lee Curtis är fantastisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new Review { MovieId = halloween.Id, UserId = emma,    Rating = 5, Comment = "Musiken. Masken. Mörkret. Perfektion.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-42) },
                new Review { MovieId = halloween.Id, UserId = klara,   Rating = 4, Comment = "Simpelt men effektivt. Carpenter är genial.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = halloween.Id, UserId = elin,    Rating = 3, Comment = "Klassiker men jag har sett bättre.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = halloween.Id, UserId = simon,   Rating = 5, Comment = "Definierade slasher-genren.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = halloween.Id, UserId = david,   Rating = 4, Comment = "Jamie Lee Curtis – scream queen nr 1.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Review { MovieId = halloween.Id, UserId = wilma,   Rating = 4, Comment = "Stämningsfull och otäck.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },

                // Hellraiser
                new Review { MovieId = hellraiser.Id, UserId = erik,    Rating = 3, Comment = "Pinhead är cool men filmen är ojämn.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new Review { MovieId = hellraiser.Id, UserId = oscar,   Rating = 3, Comment = "Intressant koncept, lite billig produktion.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new Review { MovieId = hellraiser.Id, UserId = lisa,    Rating = 2, Comment = "För gory för mig. Inte min grej.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-6) },
                new Review { MovieId = hellraiser.Id, UserId = adam,    Rating = 3, Comment = "Pinhead förtjänar en bättre film.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = hellraiser.Id, UserId = nils,    Rating = 4, Comment = "Clive Barkers vision är unik.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = hellraiser.Id, UserId = simon,   Rating = 2, Comment = "Mer äcklig än läskig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = hellraiser.Id, UserId = david,   Rating = 3, Comment = "Originell men ojämn.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = hellraiser.Id, UserId = maja,    Rating = 1, Comment = "Nej tack. Alldeles för äckigt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },
                new Review { MovieId = hellraiser.Id, UserId = saga,    Rating = 3, Comment = "Kreativt men inte för alla.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-23) },

                // The Texas Chain Saw Massacre
                new Review { MovieId = texaschainsaw.Id, UserId = erik,    Rating = 4, Comment = "Rå, smutsig och genuint skrämmande.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-17) },
                new Review { MovieId = texaschainsaw.Id, UserId = anna,    Rating = 3, Comment = "Effektiv men svår att titta på ibland.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-11) },
                new Review { MovieId = texaschainsaw.Id, UserId = viktor,  Rating = 4, Comment = "70-talsskräck när den är som bäst.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Review { MovieId = texaschainsaw.Id, UserId = jakob,   Rating = 4, Comment = "Leatherface är genuint obehaglig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = texaschainsaw.Id, UserId = nils,    Rating = 3, Comment = "Historiskt viktig men svår att se.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = texaschainsaw.Id, UserId = elin,    Rating = 2, Comment = "Orkar inte. För brutal.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = texaschainsaw.Id, UserId = david,   Rating = 4, Comment = "Smutsig och autentisk. Skrämmande.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = texaschainsaw.Id, UserId = lukas,   Rating = 3, Comment = "Respekterar den men vill inte se den igen.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },

                // Gran Torino
                new Review { MovieId = grantorino.Id, UserId = anna,    Rating = 5, Comment = "Eastwood levererar en emotionell knock-out.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new Review { MovieId = grantorino.Id, UserId = maria,   Rating = 4, Comment = "Vacker berättelse om förändring och försoning.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Review { MovieId = grantorino.Id, UserId = oscar,   Rating = 4, Comment = "Clint Eastwood i toppform. Stark avslutning.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Review { MovieId = grantorino.Id, UserId = lisa,    Rating = 3, Comment = "Bra film men lite förutsägbar.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-4) },
                new Review { MovieId = grantorino.Id, UserId = emma,    Rating = 4, Comment = "Eastwood som grumpy gubbe = perfekt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-42) },
                new Review { MovieId = grantorino.Id, UserId = klara,   Rating = 5, Comment = "Slutet förstörde mig. Underbar film.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = grantorino.Id, UserId = elin,    Rating = 4, Comment = "Överraskande emotionell. Eastwood levererar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = grantorino.Id, UserId = amanda,  Rating = 3, Comment = "Bra men lite slow burn.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = grantorino.Id, UserId = oliver,  Rating = 4, Comment = "Clints bästa film som regissör.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },

                // Cast Away
                new Review { MovieId = castaway.Id, UserId = maria,   Rating = 4, Comment = "Tom Hanks bär hela filmen ensam. Imponerande.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-19) },
                new Review { MovieId = castaway.Id, UserId = sofia,   Rating = 4, Comment = "WILSON! Gråter varje gång.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-13) },
                new Review { MovieId = castaway.Id, UserId = viktor,  Rating = 3, Comment = "Bra men lite seg i mitten.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new Review { MovieId = castaway.Id, UserId = hanna,   Rating = 4, Comment = "Hanks pratar med en volleyboll och jag gråter.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-41) },
                new Review { MovieId = castaway.Id, UserId = klara,   Rating = 3, Comment = "Imponerande men lite enformig.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = castaway.Id, UserId = nils,    Rating = 4, Comment = "Overlevnadsfilm på hög nivå.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-34) },
                new Review { MovieId = castaway.Id, UserId = amanda,  Rating = 5, Comment = "Tom Hanks bästa prestation efter Forrest Gump.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = castaway.Id, UserId = david,   Rating = 3, Comment = "Bra men jag tappade intresse på ön.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = castaway.Id, UserId = lukas,   Rating = 4, Comment = "Wilson-scenen är filmhistoriens mest absurda tårögda stund.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },

                // Jaws
                new Review { MovieId = jaws.Id, UserId = anna,    Rating = 5, Comment = "Spielbergs mästerverk. Fortfarande spännande.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-22) },
                new Review { MovieId = jaws.Id, UserId = erik,    Rating = 4, Comment = "Musiken skapar mer skräck än hajen. Genialiskt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-16) },
                new Review { MovieId = jaws.Id, UserId = oscar,   Rating = 4, Comment = "Klassiker som håller. Roy Scheider är grym.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new Review { MovieId = jaws.Id, UserId = lisa,    Rating = 3, Comment = "Lite gammal men förstår varför folk gillar den.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Review { MovieId = jaws.Id, UserId = emma,    Rating = 4, Comment = "Spielberg skapade sommarens blockbuster.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-43) },
                new Review { MovieId = jaws.Id, UserId = hanna,   Rating = 5, Comment = "You're gonna need a bigger boat. Ikoniskt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = jaws.Id, UserId = nils,    Rating = 4, Comment = "John Williams musik ÄR denna film.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = jaws.Id, UserId = maja,    Rating = 3, Comment = "Hajen ser fejk ut men stämningen bär.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = jaws.Id, UserId = saga,    Rating = 5, Comment = "Vågade inte bada efter den här. På riktigt.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-29) },

                // The Mask
                new Review { MovieId = themask.Id, UserId = sofia,   Rating = 4, Comment = "Jim Carrey på topp. Rolig och energisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new Review { MovieId = themask.Id, UserId = fredrik, Rating = 3, Comment = "Nostalgitripp men inte lika rolig som vuxen.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new Review { MovieId = themask.Id, UserId = maria,   Rating = 3, Comment = "Underhållande men ganska dum humor.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-6) },
                new Review { MovieId = themask.Id, UserId = emma,    Rating = 4, Comment = "Carrey är born for this. Energi x1000.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-42) },
                new Review { MovieId = themask.Id, UserId = klara,   Rating = 3, Comment = "Rolig men inte mitt favoritgenre.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = themask.Id, UserId = elin,    Rating = 4, Comment = "Cuban Pete-scenen! Carrey är galen.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = themask.Id, UserId = amanda,  Rating = 2, Comment = "För barnsllig. Inte min grej.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = themask.Id, UserId = lukas,   Rating = 3, Comment = "Nostalgi men inte lika rolig som jag mindes.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Review { MovieId = themask.Id, UserId = oliver,  Rating = 4, Comment = "Cameron Diaz debut – wow.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-25) },

                // The Thing
                new Review { MovieId = thething.Id, UserId = erik,    Rating = 5, Comment = "Bästa body horror-filmen. Praktiska effekter i världsklass.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-21) },
                new Review { MovieId = thething.Id, UserId = anna,    Rating = 4, Comment = "Paranoia-stämningen är fantastisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new Review { MovieId = thething.Id, UserId = oscar,   Rating = 5, Comment = "Kurt Russell + Carpenter = magi. Oöverträffad.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new Review { MovieId = thething.Id, UserId = viktor,  Rating = 4, Comment = "Obehaglig och smart. Håller fortfarande.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-4) },
                new Review { MovieId = thething.Id, UserId = jakob,   Rating = 5, Comment = "Blodscenescenan med petri-skålen – briljant!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-41) },
                new Review { MovieId = thething.Id, UserId = hanna,   Rating = 4, Comment = "Paranoia och isolation. Carpenter är mästare.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = thething.Id, UserId = nils,    Rating = 5, Comment = "Praktiska effekter som slår modern CGI.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-34) },
                new Review { MovieId = thething.Id, UserId = david,   Rating = 4, Comment = "Äckligt men briljant. En av de bästa.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = thething.Id, UserId = wilma,   Rating = 3, Comment = "Bra men lite för gory.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },

                // Bloodsport
                new Review { MovieId = bloodsport.Id, UserId = fredrik, Rating = 4, Comment = "Van Damme i sina bästa dagar. Ren kampsport-action.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new Review { MovieId = bloodsport.Id, UserId = oscar,   Rating = 3, Comment = "Kul fights men skådespeleriet är inte toppklass.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Review { MovieId = bloodsport.Id, UserId = erik,    Rating = 3, Comment = "Guilty pleasure. Dålig dialog men bra action.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Review { MovieId = bloodsport.Id, UserId = adam,    Rating = 4, Comment = "Van Damme kicks – vad mer behövs?", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = bloodsport.Id, UserId = nils,    Rating = 3, Comment = "Underhållande men dåligt manus.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = bloodsport.Id, UserId = simon,   Rating = 2, Comment = "Koreografin är bra men allt annat är sämre.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = bloodsport.Id, UserId = lukas,   Rating = 4, Comment = "Kumite! Kumite! Kultklassiker.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = bloodsport.Id, UserId = oliver,  Rating = 2, Comment = "Tråkig och förutsägbar. Van Damme sparkar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },
                new Review { MovieId = bloodsport.Id, UserId = hugo,    Rating = 3, Comment = "Nostalgi men inget mästerverk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-23) },

                // Big Trouble in Little China
                new Review { MovieId = bigtrouble.Id, UserId = viktor,  Rating = 4, Comment = "Kurt Russell är hilarious. Underskattad film.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-19) },
                new Review { MovieId = bigtrouble.Id, UserId = sofia,   Rating = 3, Comment = "Rolig men ganska rörig handling.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-13) },
                new Review { MovieId = bigtrouble.Id, UserId = fredrik, Rating = 4, Comment = "Kultklassiker! Carpenter + Russell levererar igen.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new Review { MovieId = bigtrouble.Id, UserId = jakob,   Rating = 4, Comment = "Russell som Jack Burton – briljant komedi-action.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = bigtrouble.Id, UserId = hanna,   Rating = 3, Comment = "Kul men helt galen handling.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = bigtrouble.Id, UserId = nils,    Rating = 5, Comment = "Underskattad kultklassiker. Carpenter på topp.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = bigtrouble.Id, UserId = simon,   Rating = 3, Comment = "Rolig men jag fattar inte handlingen.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = bigtrouble.Id, UserId = david,   Rating = 4, Comment = "Jack Burton är underbar. Russell i sitt esse.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = bigtrouble.Id, UserId = wilma,   Rating = 2, Comment = "Fattar inte hypen. Rörig och konstig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },

                // Platoon
                new Review { MovieId = platoon.Id, UserId = anna,    Rating = 4, Comment = "Realistisk krigsfilm. Dafoe och Sheen är fantastiska.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new Review { MovieId = platoon.Id, UserId = oscar,   Rating = 5, Comment = "Oliver Stone fångar Vietnam-krigets kaos perfekt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new Review { MovieId = platoon.Id, UserId = lisa,    Rating = 3, Comment = "Tung men viktig film. Svår att se om.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-6) },
                new Review { MovieId = platoon.Id, UserId = emma,    Rating = 4, Comment = "Dafoes dödsscen – ikonisk filmhistoria.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-42) },
                new Review { MovieId = platoon.Id, UserId = jakob,   Rating = 5, Comment = "Stone filmade sin egna upplevelse. Det märks.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-38) },
                new Review { MovieId = platoon.Id, UserId = klara,   Rating = 4, Comment = "Svår att se men viktig.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = platoon.Id, UserId = david,   Rating = 3, Comment = "Brutal och ärlig. Inte enkel underhållning.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-31) },
                new Review { MovieId = platoon.Id, UserId = saga,    Rating = 4, Comment = "Bästa Vietnamfilmen efter Apocalypse Now.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-28) },

                // Rambo: First Blood
                new Review { MovieId = rambo.Id, UserId = erik,    Rating = 4, Comment = "Mycket bättre än man tror. Stallone visar känsla.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-17) },
                new Review { MovieId = rambo.Id, UserId = fredrik, Rating = 4, Comment = "Mer drama än action. Oväntat bra.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-11) },
                new Review { MovieId = rambo.Id, UserId = viktor,  Rating = 3, Comment = "Okej men uppföljarna förstörde konceptet.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Review { MovieId = rambo.Id, UserId = adam,    Rating = 4, Comment = "Stallone visar att han kan mer än action.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = rambo.Id, UserId = nils,    Rating = 5, Comment = "PTSD och Vietnam – oväntat djup.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = rambo.Id, UserId = simon,   Rating = 3, Comment = "Bra start men serien blev sämre.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = rambo.Id, UserId = david,   Rating = 4, Comment = "Stallones monolog i slutet – riktigt bra.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = rambo.Id, UserId = lukas,   Rating = 3, Comment = "Bra drama med action-inslag.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },
                new Review { MovieId = rambo.Id, UserId = hugo,    Rating = 4, Comment = "Den enda Rambo-filmen som är på riktigt bra.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-23) },

                // RoboCop
                new Review { MovieId = robocop.Id, UserId = oscar,   Rating = 4, Comment = "Satirisk action. Verhoeven är briljant.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new Review { MovieId = robocop.Id, UserId = erik,    Rating = 4, Comment = "Blodig, rolig och smart. 80-talet i ett nötskal.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Review { MovieId = robocop.Id, UserId = sofia,   Rating = 3, Comment = "Kul koncept men lite för våldsam.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Review { MovieId = robocop.Id, UserId = jakob,   Rating = 4, Comment = "Dead or alive, you're coming with me!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-41) },
                new Review { MovieId = robocop.Id, UserId = adam,    Rating = 5, Comment = "Verhoevens satir är briljant.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = robocop.Id, UserId = nils,    Rating = 3, Comment = "Bra men väldigt våldsam.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-34) },
                new Review { MovieId = robocop.Id, UserId = david,   Rating = 4, Comment = "Samhällskritik gömd bakom action-ytan.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = robocop.Id, UserId = lukas,   Rating = 3, Comment = "Övervåldsam men med poäng.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = robocop.Id, UserId = maja,    Rating = 2, Comment = "För brutal. Inte min kopp te.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },

                // Pirates of the Caribbean
                new Review { MovieId = pirates.Id, UserId = maria,   Rating = 4, Comment = "Johnny Depp stjäl varje scen. Underhållande!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-19) },
                new Review { MovieId = pirates.Id, UserId = sofia,   Rating = 5, Comment = "Äventyr, humor och action – allt man behöver.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-13) },
                new Review { MovieId = pirates.Id, UserId = lisa,    Rating = 3, Comment = "Kul men lite för lång.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new Review { MovieId = pirates.Id, UserId = viktor,  Rating = 4, Comment = "Captain Jack Sparrow är filmhistoriens bästa pirat.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-3) },
                new Review { MovieId = pirates.Id, UserId = emma,    Rating = 5, Comment = "Depp skapade en oförglömlig karaktär.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-43) },
                new Review { MovieId = pirates.Id, UserId = hanna,   Rating = 4, Comment = "Rolig äventyrsfilm. Keira Knightley är grym.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = pirates.Id, UserId = klara,   Rating = 3, Comment = "Underhållande men lite för lång.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-35) },
                new Review { MovieId = pirates.Id, UserId = amanda,  Rating = 5, Comment = "Bästa äventyrsfilmen 2000-talet.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = pirates.Id, UserId = maja,    Rating = 4, Comment = "Piratfilm med charm och humor.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = pirates.Id, UserId = wilma,   Rating = 4, Comment = "Orlando Bloom och Depp – bra combo.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },

                // Schindler's List
                new Review { MovieId = schindlerslist.Id, UserId = anna,    Rating = 5, Comment = "Hjärtskärande och viktigt. Alla borde se den.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-22) },
                new Review { MovieId = schindlerslist.Id, UserId = maria,   Rating = 5, Comment = "Gråter okontrollerat. Liam Neeson är fantastisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-16) },
                new Review { MovieId = schindlerslist.Id, UserId = oscar,   Rating = 5, Comment = "Spielbergs bästa film. Mästerverk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new Review { MovieId = schindlerslist.Id, UserId = lisa,    Rating = 4, Comment = "Svår att se men otroligt viktig.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Review { MovieId = schindlerslist.Id, UserId = emma,    Rating = 5, Comment = "Filmhistoriens viktigaste film.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-44) },
                new Review { MovieId = schindlerslist.Id, UserId = hanna,   Rating = 5, Comment = "Neeson, Fiennes – otroliga prestationer.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = schindlerslist.Id, UserId = klara,   Rating = 5, Comment = "Flickan i rött – scenen som förstör en.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = schindlerslist.Id, UserId = elin,    Rating = 4, Comment = "Nödvändig men oerhört svår att se.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = schindlerslist.Id, UserId = amanda,  Rating = 5, Comment = "Gråter varje gång. Varje. Gång.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = schindlerslist.Id, UserId = saga,    Rating = 5, Comment = "Ren filmkonst. Spielberg vid sin allra bästa.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = schindlerslist.Id, UserId = wilma,   Rating = 5, Comment = "Alla borde se denna film minst en gång.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },

                // The Good, the Bad and the Ugly
                new Review { MovieId = goodbadugly.Id, UserId = erik,    Rating = 5, Comment = "Leones mästerverk. Slutscenen är perfektion.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-21) },
                new Review { MovieId = goodbadugly.Id, UserId = oscar,   Rating = 4, Comment = "Lång men varje scen har en poäng. Morricones musik!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new Review { MovieId = goodbadugly.Id, UserId = fredrik, Rating = 4, Comment = "Eastwood i sin mest ikoniska roll. Spaghetti-western på topp.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new Review { MovieId = goodbadugly.Id, UserId = jakob,   Rating = 5, Comment = "Morricones musik – den bästa filmmusiken.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-41) },
                new Review { MovieId = goodbadugly.Id, UserId = adam,    Rating = 4, Comment = "Eastwood, Van Cleef, Wallach – legender.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = goodbadugly.Id, UserId = nils,    Rating = 5, Comment = "Mexican standoff-scenen = filmhistoria.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-34) },
                new Review { MovieId = goodbadugly.Id, UserId = simon,   Rating = 3, Comment = "Klassiker men lite för lång.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = goodbadugly.Id, UserId = david,   Rating = 4, Comment = "Bästa western-filmen. Period.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = goodbadugly.Id, UserId = lukas,   Rating = 5, Comment = "Tuco stjäl varje scen. Eli Wallach är genial.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },

                // Cliffhanger
                new Review { MovieId = cliffhanger.Id, UserId = fredrik, Rating = 3, Comment = "Stallone klättrar runt. Underhållande men inget mer.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new Review { MovieId = cliffhanger.Id, UserId = oscar,   Rating = 3, Comment = "Bra action i bergen men tunn handling.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new Review { MovieId = cliffhanger.Id, UserId = erik,    Rating = 2, Comment = "Sämre än jag mindes. Ganska generisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-6) },
                new Review { MovieId = cliffhanger.Id, UserId = adam,    Rating = 3, Comment = "Stallone i bergen. OK men inget mer.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-39) },
                new Review { MovieId = cliffhanger.Id, UserId = nils,    Rating = 2, Comment = "Generisk 90-tals action.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = cliffhanger.Id, UserId = simon,   Rating = 3, Comment = "Öppningsscenen är bra, resten meh.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-32) },
                new Review { MovieId = cliffhanger.Id, UserId = david,   Rating = 2, Comment = "Ingen höjdpunkt (ordvitsen intended).", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-29) },
                new Review { MovieId = cliffhanger.Id, UserId = lukas,   Rating = 3, Comment = "Hyfsad action men glömbar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-26) },
                new Review { MovieId = cliffhanger.Id, UserId = oliver,  Rating = 2, Comment = "Stallone har gjort bättre.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-23) },

                // The Expendables
                new Review { MovieId = expendables.Id, UserId = oscar,   Rating = 3, Comment = "Alla action-legenderna samlade. Kul men dåligt manus.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-17) },
                new Review { MovieId = expendables.Id, UserId = erik,    Rating = 2, Comment = "Nostalgi räcker inte. Platt och förutsägbar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-11) },
                new Review { MovieId = expendables.Id, UserId = fredrik, Rating = 3, Comment = "Stäng av hjärnan och njut av explosionerna.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Review { MovieId = expendables.Id, UserId = viktor,  Rating = 2, Comment = "Besviken. Hade förväntat mig mer av det här gänget.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new Review { MovieId = expendables.Id, UserId = adam,    Rating = 2, Comment = "Alla stjärnor, noll substans.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = expendables.Id, UserId = nils,    Rating = 3, Comment = "Nostalgi-bait men jag köper det.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-36) },
                new Review { MovieId = expendables.Id, UserId = simon,   Rating = 1, Comment = "Sämsta actionfilmen jag sett. Pinsamt.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = expendables.Id, UserId = david,   Rating = 2, Comment = "Explosioner kan inte rädda dåligt manus.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = expendables.Id, UserId = lukas,   Rating = 3, Comment = "OK om man inte tänker. Annars dålig.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = expendables.Id, UserId = maja,    Rating = 1, Comment = "Tråkigt och smaklöst.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-24) },
                new Review { MovieId = expendables.Id, UserId = oliver,  Rating = 2, Comment = "Slöseri med talang.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-21) },

                // Indiana Jones and the Raiders of the Lost Ark
                new Review { MovieId = indianajones.Id, UserId = anna,    Rating = 5, Comment = "Det perfekta äventyret. Harrison Ford ÄR Indiana Jones.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-23) },
                new Review { MovieId = indianajones.Id, UserId = sofia,   Rating = 5, Comment = "Tidlöst äventyr. Musiken ger frossa!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-17) },
                new Review { MovieId = indianajones.Id, UserId = maria,   Rating = 4, Comment = "Spielberg och Ford – drömteamet.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-11) },
                new Review { MovieId = indianajones.Id, UserId = viktor,  Rating = 5, Comment = "Snakes... why did it have to be snakes? Perfekt!", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Review { MovieId = indianajones.Id, UserId = emma,    Rating = 5, Comment = "Ford med hatt och piska – filmhistoriens coolaste.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-44) },
                new Review { MovieId = indianajones.Id, UserId = hanna,   Rating = 4, Comment = "Äventyr med stor A. Spielberg levererar.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new Review { MovieId = indianajones.Id, UserId = klara,   Rating = 5, Comment = "Williams musik + Ford = magi.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-37) },
                new Review { MovieId = indianajones.Id, UserId = elin,    Rating = 4, Comment = "Rolig, spännande och nostalgisk.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-33) },
                new Review { MovieId = indianajones.Id, UserId = amanda,  Rating = 5, Comment = "Harrison Ford i sin bästa roll.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Review { MovieId = indianajones.Id, UserId = david,   Rating = 4, Comment = "Spielberg + Lucas i perfekt samarbete.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-27) },
                new Review { MovieId = indianajones.Id, UserId = saga,    Rating = 5, Comment = "Bästa äventyrsfilmen genom tiderna.", UseNickname = false, CreatedAt = DateTime.UtcNow.AddDays(-24) },
                new Review { MovieId = indianajones.Id, UserId = hugo,    Rating = 5, Comment = "Perfekt film. Inget att tillägga.", UseNickname = true, CreatedAt = DateTime.UtcNow.AddDays(-21) }
            };

            context.Reviews.AddRange(seedReviews);
            await context.SaveChangesAsync();

            // Uppdatera RatingAverage och RatingCount för alla filmer med recensioner
            var reviewedMovieIds = seedReviews.Select(r => r.MovieId).Distinct().ToList();
            foreach (var movieId in reviewedMovieIds)
            {
                var movie = await context.Movies.FirstOrDefaultAsync(m => m.Id == movieId);
                if (movie == null) continue;

                var movieReviews = await context.Reviews
                    .Where(r => r.MovieId == movieId && !r.IsDeleted)
                    .ToListAsync();

                movie.RatingCount = movieReviews.Count;
                movie.RatingAverage = movieReviews.Count == 0
                    ? 0
                    : movieReviews.Average(r => r.Rating);
            }

            await context.SaveChangesAsync();
        }

        // =========================
        // Seed: Rentals (beställningar)
        // =========================
        if (!await context.Rentals.AnyAsync())
        {
            var anna    = createdUserIds.GetValueOrDefault("anna.lindstrom@retrovhs.se");
            var erik    = createdUserIds.GetValueOrDefault("erik.johansson@retrovhs.se");
            var maria   = createdUserIds.GetValueOrDefault("maria.karlsson@retrovhs.se");
            var oscar   = createdUserIds.GetValueOrDefault("oscar.nilsson@retrovhs.se");
            var lisa    = createdUserIds.GetValueOrDefault("lisa.andersson@retrovhs.se");
            var fredrik = createdUserIds.GetValueOrDefault("fredrik.berg@retrovhs.se");
            var sofia   = createdUserIds.GetValueOrDefault("sofia.ekstrom@retrovhs.se");
            var viktor  = createdUserIds.GetValueOrDefault("viktor.larsson@retrovhs.se");
            var emma    = createdUserIds.GetValueOrDefault("emma.svensson@retrovhs.se");
            var jakob   = createdUserIds.GetValueOrDefault("jakob.pettersson@retrovhs.se");
            var adam    = createdUserIds.GetValueOrDefault("adam.gustafsson@retrovhs.se");
            var nils    = createdUserIds.GetValueOrDefault("nils.lundberg@retrovhs.se");
            var simon   = createdUserIds.GetValueOrDefault("simon.fransson@retrovhs.se");
            var david   = createdUserIds.GetValueOrDefault("david.sjoberg@retrovhs.se");
            var lukas   = createdUserIds.GetValueOrDefault("lukas.blom@retrovhs.se");

            var seedRentals = new List<Rental>
            {
                // === Beställningar under leverans (Active) ===
                new Rental { UserId = anna,   MovieId = avatar.Id,       PricePaid = 49, Status = RentalStatus.Active,    RentedAt = DateTime.UtcNow.AddDays(-2),  ExpiresAt = DateTime.UtcNow.AddDays(5)  },
                new Rental { UserId = erik,   MovieId = darkknight.Id,   PricePaid = 49, Status = RentalStatus.Active,    RentedAt = DateTime.UtcNow.AddDays(-1),  ExpiresAt = DateTime.UtcNow.AddDays(6)  },
                new Rental { UserId = maria,  MovieId = inception.Id,    PricePaid = 49, Status = RentalStatus.Active,    RentedAt = DateTime.UtcNow.AddDays(-3),  ExpiresAt = DateTime.UtcNow.AddDays(4)  },
                new Rental { UserId = oscar,  MovieId = pulp.Id,         PricePaid = 45, Status = RentalStatus.Active,    RentedAt = DateTime.UtcNow.AddDays(-1),  ExpiresAt = DateTime.UtcNow.AddDays(6)  },
                new Rental { UserId = sofia,  MovieId = godfather.Id,    PricePaid = 49, Status = RentalStatus.Active,    RentedAt = DateTime.UtcNow.AddDays(-2),  ExpiresAt = DateTime.UtcNow.AddDays(5)  },
                new Rental { UserId = viktor, MovieId = interstellar.Id, PricePaid = 49, Status = RentalStatus.Active,    RentedAt = DateTime.UtcNow.AddDays(-4),  ExpiresAt = DateTime.UtcNow.AddDays(3)  },
                new Rental { UserId = emma,   MovieId = matrix.Id,       PricePaid = 45, Status = RentalStatus.Active,    RentedAt = DateTime.UtcNow.AddDays(-1),  ExpiresAt = DateTime.UtcNow.AddDays(6)  },
                new Rental { UserId = jakob,  MovieId = goodfellas.Id,   PricePaid = 42, Status = RentalStatus.Active,    RentedAt = DateTime.UtcNow.AddDays(-3),  ExpiresAt = DateTime.UtcNow.AddDays(4)  },

                // === Levererade beställningar (Completed) ===
                new Rental { UserId = anna,    MovieId = shawshank.Id,       PricePaid = 45, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-30), ExpiresAt = DateTime.UtcNow.AddDays(-23) },
                new Rental { UserId = anna,    MovieId = fightclub.Id,       PricePaid = 42, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-60), ExpiresAt = DateTime.UtcNow.AddDays(-53) },
                new Rental { UserId = erik,    MovieId = terminator2.Id,     PricePaid = 45, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-25), ExpiresAt = DateTime.UtcNow.AddDays(-18) },
                new Rental { UserId = erik,    MovieId = alien.Id,           PricePaid = 39, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-50), ExpiresAt = DateTime.UtcNow.AddDays(-43) },
                new Rental { UserId = maria,   MovieId = forrestgump.Id,     PricePaid = 42, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-20), ExpiresAt = DateTime.UtcNow.AddDays(-13) },
                new Rental { UserId = maria,   MovieId = titanic.Id,         PricePaid = 45, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-45), ExpiresAt = DateTime.UtcNow.AddDays(-38) },
                new Rental { UserId = oscar,   MovieId = scarface.Id,        PricePaid = 39, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-35), ExpiresAt = DateTime.UtcNow.AddDays(-28) },
                new Rental { UserId = oscar,   MovieId = blood.Id,           PricePaid = 39, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-55), ExpiresAt = DateTime.UtcNow.AddDays(-48) },
                new Rental { UserId = lisa,    MovieId = schindlerslist.Id,  PricePaid = 45, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-15), ExpiresAt = DateTime.UtcNow.AddDays(-8)  },
                new Rental { UserId = lisa,    MovieId = braveheart.Id,      PricePaid = 45, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-40), ExpiresAt = DateTime.UtcNow.AddDays(-33) },
                new Rental { UserId = fredrik, MovieId = gladiator.Id,       PricePaid = 45, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-22), ExpiresAt = DateTime.UtcNow.AddDays(-15) },
                new Rental { UserId = fredrik, MovieId = diehard.Id,         PricePaid = 39, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-48), ExpiresAt = DateTime.UtcNow.AddDays(-41) },
                new Rental { UserId = sofia,   MovieId = jurassicpark.Id,    PricePaid = 45, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-18), ExpiresAt = DateTime.UtcNow.AddDays(-11) },
                new Rental { UserId = viktor,  MovieId = se7en.Id,           PricePaid = 42, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-28), ExpiresAt = DateTime.UtcNow.AddDays(-21) },
                new Rental { UserId = emma,    MovieId = savingprivateryan.Id, PricePaid = 45, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-33), ExpiresAt = DateTime.UtcNow.AddDays(-26) },
                new Rental { UserId = jakob,   MovieId = indianajones.Id,    PricePaid = 42, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-42), ExpiresAt = DateTime.UtcNow.AddDays(-35) },
                new Rental { UserId = adam,    MovieId = nocountry.Id,       PricePaid = 42, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-27), ExpiresAt = DateTime.UtcNow.AddDays(-20) },
                new Rental { UserId = nils,    MovieId = taxidriver.Id,      PricePaid = 39, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-38), ExpiresAt = DateTime.UtcNow.AddDays(-31) },
                new Rental { UserId = simon,   MovieId = shining.Id,         PricePaid = 42, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-52), ExpiresAt = DateTime.UtcNow.AddDays(-45) },
                new Rental { UserId = david,   MovieId = heat.Id,            PricePaid = 42, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-19), ExpiresAt = DateTime.UtcNow.AddDays(-12) },
                new Rental { UserId = lukas,   MovieId = rocky.Id,           PricePaid = 42, Status = RentalStatus.Completed, RentedAt = DateTime.UtcNow.AddDays(-44), ExpiresAt = DateTime.UtcNow.AddDays(-37) },

                // === Avbrutna beställningar (Cancelled) ===
                new Rental { UserId = anna,    MovieId = predator.Id,    PricePaid = 39, Status = RentalStatus.Cancelled, RentedAt = DateTime.UtcNow.AddDays(-10), ExpiresAt = DateTime.UtcNow.AddDays(-3)  },
                new Rental { UserId = erik,    MovieId = expendables.Id, PricePaid = 39, Status = RentalStatus.Cancelled, RentedAt = DateTime.UtcNow.AddDays(-14), ExpiresAt = DateTime.UtcNow.AddDays(-7)  },
                new Rental { UserId = maria,   MovieId = cliffhanger.Id, PricePaid = 39, Status = RentalStatus.Cancelled, RentedAt = DateTime.UtcNow.AddDays(-8),  ExpiresAt = DateTime.UtcNow.AddDays(-1)  },
                new Rental { UserId = oscar,   MovieId = themask.Id,     PricePaid = 35, Status = RentalStatus.Cancelled, RentedAt = DateTime.UtcNow.AddDays(-12), ExpiresAt = DateTime.UtcNow.AddDays(-5)  },
                new Rental { UserId = lisa,    MovieId = bloodsport.Id,  PricePaid = 35, Status = RentalStatus.Cancelled, RentedAt = DateTime.UtcNow.AddDays(-7),  ExpiresAt = DateTime.UtcNow.AddDays(0)   },
                new Rental { UserId = fredrik, MovieId = robocop.Id,     PricePaid = 39, Status = RentalStatus.Cancelled, RentedAt = DateTime.UtcNow.AddDays(-9),  ExpiresAt = DateTime.UtcNow.AddDays(-2)  },
                new Rental { UserId = sofia,   MovieId = halloween.Id,   PricePaid = 35, Status = RentalStatus.Cancelled, RentedAt = DateTime.UtcNow.AddDays(-16), ExpiresAt = DateTime.UtcNow.AddDays(-9)  },
                new Rental { UserId = adam,    MovieId = casino.Id,      PricePaid = 42, Status = RentalStatus.Cancelled, RentedAt = DateTime.UtcNow.AddDays(-11), ExpiresAt = DateTime.UtcNow.AddDays(-4)  },
            };

            context.Rentals.AddRange(seedRentals);
            await context.SaveChangesAsync();
        }
    }
}