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
    }
}