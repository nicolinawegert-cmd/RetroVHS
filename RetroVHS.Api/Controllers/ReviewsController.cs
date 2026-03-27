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

    var userId = GetUserId();

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

  /// <summary>
  /// Uppdaterar en befintlig recension för den inloggade användaren.
  /// </summary>
  [Authorize]
  [HttpPut("{id:int}")]
  public async Task<ActionResult<ReviewDto>> UpdateReview(int id, [FromBody] UpdateReviewDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    if (id != dto.Id)
      return BadRequest(new { message = "Route id och dto.Id matchar inte." });

    var userId = GetUserId();

    var review = await _reviewService.UpdateReviewAsync(userId, id, dto);

    if (review == null)
      return NotFound();

    return Ok(review);
  }

  /// <summary>
  /// Tar bort den aktuella användarens recension/kommentar.
  /// </summary>
  [Authorize]
  [HttpDelete("{id:int}")]
  public async Task<IActionResult> RemoveReviewComment(int id)
  {
    var userId = GetUserId();

    var deleted = await _reviewService.RemoveReviewCommentAsync(userId, id);

    if (!deleted)
      return NotFound();

    return NoContent();
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
