using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroVHS.Api.Services.Wishlists;
using RetroVHS.Shared.DTOs.Wishlist;

namespace RetroVHS.Api.Controllers;

/// <summary>
/// Hanterar endpoints för användarens önskelista.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
  private readonly IWishlistService _wishlistService;

  public WishlistController(IWishlistService wishlistService)
  {
    _wishlistService = wishlistService;
  }

  /// <summary>
  /// Hämtar den inloggade användarens önskelista.
  /// GET /api/wishlist
  /// </summary>
  [HttpGet]
  public async Task<ActionResult<List<WishlistItemDto>>> GetWishlist()
  {
    var userId = GetUserId();
    var items = await _wishlistService.GetWishlistAsync(userId);
    return Ok(items);
  }

  /// <summary>
  /// Lägger till en film i önskelistan.
  /// POST /api/wishlist
  /// </summary>
  [HttpPost]
  public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistDto dto)
  {
    var userId = GetUserId();
    var (success, message) = await _wishlistService.AddToWishlistAsync(userId, dto.MovieId);

    if (!success)
      return BadRequest(new { message });

    return Ok(new { message });
  }

  /// <summary>
  /// Tar bort en film från önskelistan.
  /// DELETE /api/wishlist/{movieId}
  /// </summary>
  [HttpDelete("{movieId:int}")]
  public async Task<IActionResult> RemoveFromWishlist(int movieId)
  {
    var userId = GetUserId();
    var (success, message) = await _wishlistService.RemoveFromWishlistAsync(userId, movieId);

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
