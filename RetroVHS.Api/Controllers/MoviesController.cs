using Microsoft.AspNetCore.Mvc;
using RetroVHS.Api.Services.Movies;
using RetroVHS.Shared.DTOs.Movies;

namespace RetroVHS.Api.Controllers;

/// <summary>
/// Hanterar endpoints för filmer.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet]
    public async Task<ActionResult<List<MovieListDto>>> GetMovies([FromQuery] MovieFilterDto filter)
    {
        var movies = await _movieService.GetMoviesAsync(filter);
        return Ok(movies);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieDetailsDto>> GetMovieById(int id)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);

        if (movie == null)
            return NotFound();

        return Ok(movie);
    }

}