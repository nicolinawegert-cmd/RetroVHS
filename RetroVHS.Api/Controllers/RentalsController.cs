using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroVHS.Api.Services.Rentals;

namespace RetroVHS.Api.Controllers;

/// <summary>
/// Hanterar endpoints för beställningar (rentals).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RentalsController : ControllerBase
{
  private readonly IRentalService _rentalService;

  public RentalsController(IRentalService rentalService)
  {
    _rentalService = rentalService;
  }

  /// <summary>
  /// Markerar en beställning som slutförd.
  /// Användaren kan slutföra sin egen, admin kan slutföra vilken som helst.
  /// PUT /api/rentals/{id}/complete
  /// </summary>
  [HttpPut("{id:int}/complete")]
  public async Task<IActionResult> CompleteRental(int id)
  {
    var userId = GetUserId();
    var isAdmin = User.IsInRole("Admin");

    var (success, message) = await _rentalService.CompleteRentalAsync(id, userId, isAdmin);

    if (!success)
      return BadRequest(new { message });

    return Ok(new { message });
  }

  /// <summary>
  /// Avbryter en beställning. Endast admin.
  /// PUT /api/rentals/{id}/cancel
  /// </summary>
  [Authorize(Roles = "Admin")]
  [HttpPut("{id:int}/cancel")]
  public async Task<IActionResult> CancelRental(int id)
  {
    var (success, message) = await _rentalService.CancelRentalAsync(id);

    if (!success)
      return BadRequest(new { message });

    return Ok(new { message });
  }

  /// <summary>
  /// Raderar en beställning.
  /// Användaren kan radera sin egen om den är Cancelled, admin kan radera vilken som helst.
  /// DELETE /api/rentals/{id}
  /// </summary>
  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeleteRental(int id)
  {
    var userId = GetUserId();
    var isAdmin = User.IsInRole("Admin");

    var (success, message) = await _rentalService.DeleteRentalAsync(id, userId, isAdmin);

    if (!success)
      return BadRequest(new { message });

    return Ok(new { message });
  }

  /// <summary>
  /// Hämtar användar-id från JWT-token.
  /// </summary>
  private int GetUserId()
  {
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("Användaren saknar id i token.");

    return int.Parse(userIdClaim);
  }
}
