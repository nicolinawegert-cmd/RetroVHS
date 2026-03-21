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

    /// <summary>
    /// Skapar en ny instans av controllern och injicerar movie-servicen.
    /// </summary>
    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    /// <summary>
    /// Hämtar filmer från katalogen med stöd för sökning, filtrering och sortering.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MovieListDto>>> GetMovies([FromQuery] MovieFilterDto filter)
    {
        var movies = await _movieService.GetMoviesAsync(filter);
        return Ok(movies);
    }

    /// <summary>
    /// Hämtar fullständig information om en specifik film.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieDetailsDto>> GetMovieById(int id)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);

        if (movie == null)
            return NotFound();

        return Ok(movie);
    }

    /// <summary>
    /// Skapar en ny film i katalogen. Endast administratörer har åtkomst.
    /// </summary>
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

    /// <summary>
    /// Uppdaterar en befintlig film och dess relationer. Endast administratörer har åtkomst.
    /// </summary>
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

    /// <summary>
    /// Tar bort en film från katalogen. Endast administratörer har åtkomst.
    /// </summary>
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
