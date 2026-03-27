using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroVHS.Api.Services.Admin;
using RetroVHS.Api.Services.Movies;
using RetroVHS.Shared.DTOs.Admin;
using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Movies;
using RetroVHS.Shared.DTOs.Rentals;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Api.Controllers;

/// <summary>
/// Dedikerad controller för alla administrativa funktioner.
/// Alla endpoints kräver Admin-rollen.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IMovieService _movieService;

    public AdminController(IAdminService adminService, IMovieService movieService)
    {
        _adminService = adminService;
        _movieService = movieService;
    }

    // ========== Dashboard ==========

    /// <summary>
    /// Hämtar övergripande statistik för admin-dashboarden.
    /// GET /api/admin/stats
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<AdminDashboardDto>> GetDashboardStats()
    {
        var stats = await _adminService.GetDashboardStatsAsync();
        return Ok(stats);
    }

    // ========== Användare ==========

    /// <summary>
    /// Hämtar alla användare.
    /// GET /api/admin/users
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Hämtar en specifik användare.
    /// GET /api/admin/users/{id}
    /// </summary>
    [HttpGet("users/{id:int}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _adminService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    /// <summary>
    /// Raderar en användare och all relaterad data.
    /// DELETE /api/admin/users/{id}
    /// </summary>
    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var (success, message) = await _adminService.DeleteUserAsync(id);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    /// <summary>
    /// Blockerar en användare.
    /// PUT /api/admin/users/{id}/block
    /// </summary>
    [HttpPut("users/{id:int}/block")]
    public async Task<IActionResult> BlockUser(int id)
    {
        var (success, message) = await _adminService.BlockUserAsync(id);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    /// <summary>
    /// Avblockerar en användare.
    /// PUT /api/admin/users/{id}/unblock
    /// </summary>
    [HttpPut("users/{id:int}/unblock")]
    public async Task<IActionResult> UnblockUser(int id)
    {
        var (success, message) = await _adminService.UnblockUserAsync(id);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    /// <summary>
    /// Sätter ett nytt nickname på en användare.
    /// PUT /api/admin/users/{id}/nickname
    /// </summary>
    [HttpPut("users/{id:int}/nickname")]
    public async Task<IActionResult> UpdateNickname(int id, [FromBody] AdminSetNicknameDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (success, message) = await _adminService.UpdateNicknameAsync(id, dto);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    // ========== Användarens recensioner ==========

    /// <summary>
    /// Hämtar alla recensioner för en specifik användare.
    /// GET /api/admin/users/{id}/reviews
    /// </summary>
    [HttpGet("users/{id:int}/reviews")]
    public async Task<ActionResult<List<ReviewDto>>> GetUserReviews(int id)
    {
        var reviews = await _adminService.GetUserReviewsAsync(id);
        return Ok(reviews);
    }

    /// <summary>
    /// Tar bort kommentartexten men behåller betyget.
    /// DELETE /api/admin/reviews/{id}/comment
    /// </summary>
    [HttpDelete("reviews/{id:int}/comment")]
    public async Task<IActionResult> RemoveReviewComment(int id)
    {
        var (success, message) = await _adminService.RemoveReviewCommentAsync(id);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    /// <summary>
    /// Raderar en recension helt (soft-delete).
    /// DELETE /api/admin/reviews/{id}
    /// </summary>
    [HttpDelete("reviews/{id:int}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var (success, message) = await _adminService.DeleteReviewAsync(id);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    // ========== Användarens beställningar ==========

    /// <summary>
    /// Hämtar alla beställningar för en specifik användare.
    /// GET /api/admin/users/{id}/rentals
    /// </summary>
    [HttpGet("users/{id:int}/rentals")]
    public async Task<ActionResult<List<RentalDto>>> GetUserRentals(int id)
    {
        var rentals = await _adminService.GetUserRentalsAsync(id);
        return Ok(rentals);
    }

    /// <summary>
    /// Avbryter en aktiv beställning.
    /// PUT /api/admin/rentals/{id}/cancel
    /// </summary>
    [HttpPut("rentals/{id:int}/cancel")]
    public async Task<IActionResult> CancelRental(int id)
    {
        var (success, message) = await _adminService.CancelRentalAsync(id);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    // ========== Filmer ==========

    /// <summary>
    /// Hämtar alla filmer.
    /// GET /api/admin/movies
    /// </summary>
    [HttpGet("movies")]
    public async Task<ActionResult<List<MovieListDto>>> GetAllMovies()
    {
        var movies = await _movieService.GetMoviesAsync(new MovieFilterDto());
        return Ok(movies);
    }

    /// <summary>
    /// Hämtar en specifik film med detaljer.
    /// GET /api/admin/movies/{id}
    /// </summary>
    [HttpGet("movies/{id:int}")]
    public async Task<ActionResult<MovieDetailsDto>> GetMovieById(int id)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);
        if (movie == null) return NotFound();
        return Ok(movie);
    }

    /// <summary>
    /// Skapar en ny film.
    /// POST /api/admin/movies
    /// </summary>
    [HttpPost("movies")]
    public async Task<ActionResult<MovieDetailsDto>> CreateMovie([FromBody] CreateMovieDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

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
    /// Uppdaterar en befintlig film.
    /// PUT /api/admin/movies/{id}
    /// </summary>
    [HttpPut("movies/{id:int}")]
    public async Task<ActionResult<MovieDetailsDto>> UpdateMovie(int id, [FromBody] UpdateMovieDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (id != dto.Id) return BadRequest(new { message = "Route id och dto.Id matchar inte." });

        try
        {
            var updated = await _movieService.UpdateMovieAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tar bort en film.
    /// DELETE /api/admin/movies/{id}
    /// </summary>
    [HttpDelete("movies/{id:int}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        try
        {
            var deleted = await _movieService.DeleteMovieAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
