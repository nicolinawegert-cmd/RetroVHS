using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Shared.DTOs.Movies;
using RetroVHS.Shared.Enums;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Api.Services.Movies;

/// <summary>
/// Service som hanterar affärslogik relaterad till filmer.
/// </summary>

public class MovieService : IMovieService
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Skapar en ny instans av servicen och injicerar databaskontexten.
  /// </summary>
  public MovieService(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Hämtar filmer från katalogen med stöd för sökning, filtrering och sortering.
  /// </summary>
  public async Task<List<MovieListDto>> GetMoviesAsync(MovieFilterDto filter)
  {
    var query = _context.Movies
        .Include(m => m.MovieGenres)
        .ThenInclude(mg => mg.Genre)
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
    {
      query = query.Where(m => EF.Functions.Like(m.Title, $"%{filter.SearchTerm}%"));
    }

    if (filter.Featured.HasValue)
    {
      query = query.Where(m => m.IsFeatured == filter.Featured.Value);
    }

    if (filter.GenreId.HasValue)
    {
      query = query.Where(m => m.MovieGenres.Any(mg => mg.GenreId == filter.GenreId.Value));
    }

    if (filter.ReleaseYear.HasValue)
    {
      query = query.Where(m => m.ReleaseYear == filter.ReleaseYear.Value);
    }

    if (filter.MinPrice.HasValue)
    {
      query = query.Where(m => m.RentalPrice >= filter.MinPrice.Value);
    }

    if (filter.MaxPrice.HasValue)
    {
      query = query.Where(m => m.RentalPrice <= filter.MaxPrice.Value);
    }

    if (filter.MinRating.HasValue)
    {
      query = query.Where(m => m.RatingAverage >= filter.MinRating.Value);
    }

    if (filter.AvailabilityStatus.HasValue)
    {
      query = query.Where(m => m.AvailabilityStatus == filter.AvailabilityStatus.Value);
    }

    query = filter.SortBy?.ToLowerInvariant() switch
    {
      "price" => filter.Desc
          ? query.OrderByDescending(m => m.RentalPrice)
          : query.OrderBy(m => m.RentalPrice),

      "rating" => filter.Desc
          ? query.OrderByDescending(m => m.RatingAverage)
          : query.OrderBy(m => m.RatingAverage),

      "year" => filter.Desc
          ? query.OrderByDescending(m => m.ReleaseYear)
          : query.OrderBy(m => m.ReleaseYear),

      _ => filter.Desc
          ? query.OrderByDescending(m => m.Title)
          : query.OrderBy(m => m.Title)
    };

    var movies = await query
        .Select(m => new MovieListDto
        {
          Id = m.Id,
          Title = m.Title,
          ReleaseYear = m.ReleaseYear,
          DurationMinutes = m.DurationMinutes,
          RentalPrice = m.RentalPrice,
          RatingAverage = m.RatingAverage,
          RatingCount = m.RatingCount,
          PosterUrl = m.PosterUrl,
          AvailabilityStatus = m.AvailabilityStatus.ToString(),
          IsFeatured = m.IsFeatured,
          Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList()
        })
        .ToListAsync();

    return movies;
  }

  /// <summary>
  /// Hämtar fullständig information om en film inklusive genres, credits och grunddata.
  /// </summary>
  public async Task<MovieDetailsDto?> GetMovieByIdAsync(int id)
  {
    var movie = await _context.Movies
        .Include(m => m.ProductionCompany)
        .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
        .Include(m => m.MovieCredits)
            .ThenInclude(mc => mc.Person)
        .Include(m => m.Reviews)
            .ThenInclude(r => r.User)
        .FirstOrDefaultAsync(m => m.Id == id);

    if (movie == null)
      return null;

    return new MovieDetailsDto
    {
      Id = movie.Id,
      Title = movie.Title,
      Synopsis = movie.Synopsis,
      ReleaseYear = movie.ReleaseYear,
      DurationMinutes = movie.DurationMinutes,
      RentalPrice = movie.RentalPrice,
      RatingAverage = movie.RatingAverage,
      RatingCount = movie.RatingCount,
      PosterUrl = movie.PosterUrl,
      TrailerUrl = movie.TrailerUrl,
      AvailabilityStatus = movie.AvailabilityStatus.ToString(),
      StockQuantity = movie.StockQuantity,
      IsFeatured = movie.IsFeatured,
      Language = movie.Language,
      Country = movie.Country,
      ProductionCompanyId = movie.ProductionCompanyId,
      ProductionCompanyName = movie.ProductionCompany?.Name,
      Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),

      Directors = movie.MovieCredits
          .Where(mc => mc.Role == CreditRole.Director)
          .OrderBy(mc => mc.DisplayOrder)
          .Select(mc => new PersonCreditDto
          {
            PersonId = mc.PersonId,
            FullName = mc.Person.FullName,
            Role = mc.Role.ToString(),
            CharacterName = mc.CharacterName,
            DisplayOrder = mc.DisplayOrder
          })
          .ToList(),

      Cast = movie.MovieCredits
          .Where(mc => mc.Role == CreditRole.Actor)
          .OrderBy(mc => mc.DisplayOrder)
          .Select(mc => new PersonCreditDto
          {
            PersonId = mc.PersonId,
            FullName = mc.Person.FullName,
            Role = mc.Role.ToString(),
            CharacterName = mc.CharacterName,
            DisplayOrder = mc.DisplayOrder
          })
          .ToList(),

      Reviews = movie.Reviews
          .Where(r => !r.IsDeleted)
          .OrderByDescending(r => r.CreatedAt)
          .Select(r => new ReviewDto
          {
            Id = r.Id,
            MovieId = r.MovieId,
            UserId = r.UserId,
            UserDisplayName = r.UseNickname && !string.IsNullOrWhiteSpace(r.User.Nickname)
            ? r.User.Nickname!
            : r.User.FullName,
            Comment = r.Comment ?? string.Empty,
            Rating = r.Rating,
            CreatedAt = r.CreatedAt,
            IsEdited = r.IsEdited
          })
          .ToList()
    };
  }

  /// <summary>
  /// Skapar en ny film och sparar kopplingar till genres och credits.
  /// </summary>
  public async Task<MovieDetailsDto> CreateMovieAsync(CreateMovieDto dto)
  {
    await ValidateMovieRelationsAsync(
    dto.ProductionCompanyId,
    dto.GenreIds,
    dto.Credits);

    var movie = new Movie
    {
      Title = dto.Title,
      Synopsis = dto.Synopsis,
      ReleaseYear = dto.ReleaseYear,
      DurationMinutes = dto.DurationMinutes,
      RentalPrice = dto.RentalPrice,
      PosterUrl = dto.PosterUrl,
      TrailerUrl = dto.TrailerUrl,
      Language = dto.Language,
      Country = dto.Country,
      ProductionCompanyId = dto.ProductionCompanyId,
      AvailabilityStatus = dto.AvailabilityStatus,
      StockQuantity = dto.StockQuantity,
      IsFeatured = dto.IsFeatured
    };

    _context.Movies.Add(movie);
    await _context.SaveChangesAsync();

    foreach (var genreId in dto.GenreIds.Distinct())
    {
      _context.MovieGenres.Add(new MovieGenre
      {
        MovieId = movie.Id,
        GenreId = genreId
      });
    }

    foreach (var credit in dto.Credits)
    {
      _context.MovieCredits.Add(new MovieCredit
      {
        MovieId = movie.Id,
        PersonId = credit.PersonId,
        Role = credit.Role,
        CharacterName = credit.CharacterName,
        DisplayOrder = credit.DisplayOrder
      });
    }

    await _context.SaveChangesAsync();

    return await GetMovieByIdAsync(movie.Id)
        ?? throw new InvalidOperationException("Movie could not be loaded.");
  }

  /// <summary>
  /// Uppdaterar en befintlig film och ersätter dess genres och credits med nya värden.
  /// </summary>
  public async Task<MovieDetailsDto?> UpdateMovieAsync(int id, UpdateMovieDto dto)
  {
    await ValidateMovieRelationsAsync(
    dto.ProductionCompanyId,
    dto.GenreIds,
    dto.Credits);

    var movie = await _context.Movies
    .Include(m => m.MovieGenres)
    .Include(m => m.MovieCredits)
    .FirstOrDefaultAsync(m => m.Id == id);

    if (movie == null)
      return null;

    movie.Title = dto.Title;
    movie.Synopsis = dto.Synopsis;
    movie.ReleaseYear = dto.ReleaseYear;
    movie.DurationMinutes = dto.DurationMinutes;
    movie.RentalPrice = dto.RentalPrice;
    movie.PosterUrl = dto.PosterUrl;
    movie.TrailerUrl = dto.TrailerUrl;
    movie.Language = dto.Language;
    movie.Country = dto.Country;
    movie.ProductionCompanyId = dto.ProductionCompanyId;
    movie.AvailabilityStatus = dto.AvailabilityStatus;
    movie.StockQuantity = dto.StockQuantity;
    movie.IsFeatured = dto.IsFeatured;

    _context.MovieGenres.RemoveRange(movie.MovieGenres);

    foreach (var genreId in dto.GenreIds.Distinct())
    {
      _context.MovieGenres.Add(new MovieGenre
      {
        MovieId = movie.Id,
        GenreId = genreId
      });
    }

    _context.MovieCredits.RemoveRange(movie.MovieCredits);

    foreach (var credit in dto.Credits)
    {
      _context.MovieCredits.Add(new MovieCredit
      {
        MovieId = movie.Id,
        PersonId = credit.PersonId,
        Role = credit.Role,
        CharacterName = credit.CharacterName,
        DisplayOrder = credit.DisplayOrder
      });
    }


    await _context.SaveChangesAsync();

    return await GetMovieByIdAsync(movie.Id);
  }

  /// <summary>
  /// Tar bort en film från katalogen om den finns.
  /// Relaterade ordrar och kundvagnsrader tas bort automatiskt.
  /// </summary>
  public async Task<bool> DeleteMovieAsync(int id)
  {
    var movie = await _context.Movies
        .Include(m => m.Rentals)
        .Include(m => m.CartItems)
        .FirstOrDefaultAsync(m => m.Id == id);

    if (movie == null)
      return false;

    _context.Rentals.RemoveRange(movie.Rentals);
    _context.CartItems.RemoveRange(movie.CartItems);
    _context.Movies.Remove(movie);
    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<List<MovieListDto>> GetTopRatedAsync()
  {
    var movies = await _context.Movies
        .Where(m => m.RatingCount > 0)
        .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
        .OrderByDescending(m => m.RatingAverage)
        .Take(5)
        .Select(MovieListProjection)
        .ToListAsync();

    return await FillToFive(movies, []);
  }

  public async Task<List<MovieListDto>> GetBestsellersAsync()
  {
    var topMovieIds = await _context.Rentals
        .GroupBy(r => r.MovieId)
        .OrderByDescending(g => g.Count())
        .Take(5)
        .Select(g => g.Key)
        .ToListAsync();

    var movies = await _context.Movies
        .Where(m => topMovieIds.Contains(m.Id))
        .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
        .Select(MovieListProjection)
        .ToListAsync();

    var result = movies.OrderBy(m => topMovieIds.IndexOf(m.Id)).ToList();
    return await FillToFive(result, []);
  }

  public async Task<List<GenreSectionDto>> GetTopGenreSectionsAsync()
  {
    var topGenreIds = await _context.Rentals
        .Join(_context.MovieGenres,
            r => r.MovieId,
            mg => mg.MovieId,
            (r, mg) => mg.GenreId)
        .GroupBy(gid => gid)
        .OrderByDescending(g => g.Count())
        .Take(5)
        .Select(g => g.Key)
        .ToListAsync();

    var genreNames = await _context.Genres
        .Where(g => topGenreIds.Contains(g.Id))
        .ToDictionaryAsync(g => g.Id, g => g.Name);

    var sections = new List<GenreSectionDto>();
    foreach (var genreId in topGenreIds)
    {
      if (!genreNames.TryGetValue(genreId, out var genreName)) continue;

      var movies = await _context.Movies
          .Where(m => m.MovieGenres.Any(mg => mg.GenreId == genreId))
          .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
          .OrderByDescending(m => m.RatingAverage)
          .Take(5)
          .Select(MovieListProjection)
          .ToListAsync();

      movies = await FillToFive(movies, [genreId]);

      sections.Add(new GenreSectionDto { GenreId = genreId, GenreName = genreName, Movies = movies });
    }
    return sections;
  }

  // Fyller listan till 5 filmer med slumpmässigt valda filmer som inte redan finns med.
  // Om genreIds anges begränsas fyllningen till filmer i de genrerna.
  private async Task<List<MovieListDto>> FillToFive(List<MovieListDto> existing, List<int> genreIds)
  {
    if (existing.Count >= 5) return existing;

    var excludeIds = existing.Select(m => m.Id).ToList();
    var query = _context.Movies
        .Where(m => !excludeIds.Contains(m.Id))
        .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre);

    IQueryable<Movie> filtered = genreIds.Count > 0
        ? query.Where(m => m.MovieGenres.Any(mg => genreIds.Contains(mg.GenreId)))
        : query;

    var fill = await filtered
        .OrderBy(m => m.Title)
        .Take(5 - existing.Count)
        .Select(MovieListProjection)
        .ToListAsync();

    existing.AddRange(fill);
    return existing;
  }

  private static readonly System.Linq.Expressions.Expression<Func<Movie, MovieListDto>> MovieListProjection = m => new MovieListDto
  {
    Id = m.Id, Title = m.Title, ReleaseYear = m.ReleaseYear,
    DurationMinutes = m.DurationMinutes, RentalPrice = m.RentalPrice,
    RatingAverage = m.RatingAverage, RatingCount = m.RatingCount,
    PosterUrl = m.PosterUrl, AvailabilityStatus = m.AvailabilityStatus.ToString(),
    IsFeatured = m.IsFeatured, Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList()
  };

  /// <summary>
  /// Validerar att relaterade id:n för produktionsbolag, genres och personer finns i databasen.
  /// </summary>
  private async Task ValidateMovieRelationsAsync(
    int? productionCompanyId,
    List<int> genreIds,
    List<CreateMovieCreditDto> credits)
  {
    if (productionCompanyId.HasValue)
    {
      var companyExists = await _context.ProductionCompanies
          .AnyAsync(pc => pc.Id == productionCompanyId.Value);

      if (!companyExists)
      {
        throw new ArgumentException("Ogiltigt ProductionCompanyId.");
      }
    }

    var distinctGenreIds = genreIds.Distinct().ToList();

    if (distinctGenreIds.Count > 0)
    {
      var existingGenreIds = await _context.Genres
          .Where(g => distinctGenreIds.Contains(g.Id))
          .Select(g => g.Id)
          .ToListAsync();

      if (existingGenreIds.Count != distinctGenreIds.Count)
      {
        throw new ArgumentException("En eller flera GenreIds är ogiltiga.");
      }
    }

    var distinctPersonIds = credits
        .Select(c => c.PersonId)
        .Distinct()
        .ToList();

    if (distinctPersonIds.Count > 0)
    {
      var existingPersonIds = await _context.Persons
          .Where(p => distinctPersonIds.Contains(p.Id))
          .Select(p => p.Id)
          .ToListAsync();

      if (existingPersonIds.Count != distinctPersonIds.Count)
      {
        throw new ArgumentException("En eller flera PersonId i Credits är ogiltiga.");
      }
    }
  }
}
