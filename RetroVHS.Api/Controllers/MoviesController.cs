using Microsoft.AspNetCore.Mvc;
using RetroVHS.Api.Services.Movies;
using RetroVHS.Shared.DTOs.Movies;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<MovieListDto>>> GetMovies([FromQuery] MovieFilterDto filter)
    {
        var movies = await _movieService.GetMoviesAsync(filter);
        return Ok(movies);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieDetailsDto>> GetMovieById(int id)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);

        if (movie == null)
            return NotFound();

        return Ok(movie);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<MovieDetailsDto>> CreateMovie([FromBody] CreateMovieDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _movieService.CreateMovieAsync(dto);
            return CreatedAtAction(nameof(GetMovieById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<MovieDetailsDto>> UpdateMovie(int id, [FromBody] UpdateMovieDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != dto.Id)
            return BadRequest(new { message = "Route id och dto.Id matchar inte." });

        try
        {
            var updated = await _movieService.UpdateMovieAsync(id, dto);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var deleted = await _movieService.DeleteMovieAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}