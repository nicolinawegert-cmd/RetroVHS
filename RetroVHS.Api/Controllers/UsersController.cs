using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroVHS.Api.Services.Users;
using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Reviews;


namespace RetroVHS.Api.Controllers;

/// <summary>
/// Hanterar endpoints för användarprofiler.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
  private readonly IUserService _userService;

  /// <summary>
  /// Skapar en ny instans av controllern och injicerar user-servicen.
  /// </summary>
  public UsersController(IUserService userService)
  {
    _userService = userService;
  }

  /// <summary>
  /// Hämtar profilinformationen för den inloggade användaren.
  /// </summary>
  [Authorize]
  [HttpGet("me")]
  public async Task<ActionResult<UserDto>> GetCurrentUser()
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrWhiteSpace(userIdClaim))
      return Unauthorized();

    if (!int.TryParse(userIdClaim, out var userId))
      return Unauthorized();

    var user = await _userService.GetCurrentUserAsync(userId);

    if (user == null)
      return NotFound();

    return Ok(user);
  }

  /// <summary>
  /// Uppdaterar profilinformationen för den inloggade användaren.
  /// </summary>
  [Authorize]
  [HttpPut("me")]
  public async Task<ActionResult<UserDto>> UpdateCurrentUser([FromBody] UpdateUserProfileDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrWhiteSpace(userIdClaim))
      return Unauthorized();

    if (!int.TryParse(userIdClaim, out var userId))
      return Unauthorized();

    var updatedUser = await _userService.UpdateCurrentUserAsync(userId, dto);

    if (updatedUser == null)
      return NotFound();

    return Ok(updatedUser);
  }

  /// <summary>
  /// Byter lösenord för den inloggade användaren.
  /// </summary>
  [Authorize]
  [HttpPut("me/password")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrWhiteSpace(userIdClaim))
      return Unauthorized();

    if (!int.TryParse(userIdClaim, out var userId))
      return Unauthorized();

    var result = await _userService.ChangePasswordAsync(userId, dto);

    if (!result.Succeeded)
    {
      return BadRequest(new { errors = result.Errors });
    }

    return Ok(new { message = "Lösenordet har uppdaterats." });
  }

  /// <summary>
  /// Hämtar alla recensioner som den inloggade användaren har skrivit.
  /// </summary>
  [Authorize]
  [HttpGet("me/reviews")]
  public async Task<ActionResult<List<ReviewDto>>> GetCurrentUserReviews()
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrWhiteSpace(userIdClaim))
      return Unauthorized();

    if (!int.TryParse(userIdClaim, out var userId))
      return Unauthorized();

    var reviews = await _userService.GetCurrentUserReviewsAsync(userId);

    return Ok(reviews);
  }

  /// <summary>
  /// Hämtar profilinformationen för en specifik användare. Endast administratörer har åtkomst.
  /// </summary>
  [Authorize(Roles = "Admin")]
  [HttpGet("{id:int}")]
  public async Task<ActionResult<UserDto>> GetUserById(int id)
  {
    var user = await _userService.GetUserByIdAsync(id);

    if (user == null)
      return NotFound();

    return Ok(user);
  }

}
