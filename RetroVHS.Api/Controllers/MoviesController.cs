using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Shared.DTOs.Movies;

namespace RetroVHS.Api.Controllers;

/// <summary>
/// Hanterar endpoints för filmer.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MoviesController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Hämtar filmer med valfri filtrering.
    /// Om searchTerm anges filtreras på titel.
    /// Om featured=true returneras bara featured-filmer.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MovieListDto>>> GetMovies(
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? featured = null)
    {
        var query = _context.Movies
            .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(m => EF.Functions.Like(m.Title, $"%{searchTerm}%"));
        }

        if (featured == true)
        {
            query = query.Where(m => m.IsFeatured);
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

        return Ok(movies);
    }
}
