using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroVHS.Api.Services.Reviews;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Api.Controllers;

/// <summary>
/// Hanterar endpoints för recensioner och betyg.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
  private readonly IReviewService _reviewService;

  /// <summary>
  /// Skapar en ny instans av controllern och injicerar review-servicen.
  /// </summary>
  public ReviewsController(IReviewService reviewService)
  {
    _reviewService = reviewService;
  }

  /// <summary>
  /// Skapar en ny recension för den inloggade användaren.
  /// </summary>
  [Authorize]
  [HttpPost]
  public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrWhiteSpace(userIdClaim))
      return Unauthorized();

    if (!int.TryParse(userIdClaim, out var userId))
      return Unauthorized();

    try
    {
      var review = await _reviewService.CreateReviewAsync(userId, dto);

      if (review == null)
        return NotFound();

      return Ok(review);
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }
}
