using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
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
}