using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Shared.DTOs.Movies;

namespace RetroVHS.Api.Services.Movies;

/// <summary>
/// Service som hanterar affärslogik relaterad till filmer.
/// </summary>

public class MovieService : IMovieService
{
  private readonly ApplicationDbContext _context;

  public MovieService(ApplicationDbContext context)
  {
    _context = context;
  }

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

    var movies = await query
        .OrderBy(m => m.Title)
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

  public async Task<MovieDetailsDto?> GetMovieByIdAsync(int id)
  {
    var movie = await _context.Movies
        .Include(m => m.ProductionCompany)
        .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
        .Include(m => m.MovieCredits)
            .ThenInclude(mc => mc.Person)
        .Include(m => m.Reviews)
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
      ProductionCompanyName = movie.ProductionCompany?.Name,
      Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList()
    };
  }

  public async Task<MovieDetailsDto> CreateMovieAsync(CreateMovieDto dto)
  {
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

    return await GetMovieByIdAsync(movie.Id)
        ?? throw new InvalidOperationException("Movie could not be loaded.");
  }

  public async Task<MovieDetailsDto?> UpdateMovieAsync(int id, UpdateMovieDto dto)
  {
    var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);

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

    await _context.SaveChangesAsync();

    return await GetMovieByIdAsync(movie.Id);
  }

  public async Task<bool> DeleteMovieAsync(int id)
  {
    var movie = await _context.Movies.FindAsync(id);

    if (movie == null)
      return false;

    _context.Movies.Remove(movie);
    await _context.SaveChangesAsync();
    return true;
  }

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