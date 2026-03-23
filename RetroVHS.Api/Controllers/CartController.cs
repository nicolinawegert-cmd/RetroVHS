using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroVHS.Api.Services.Cart;
using RetroVHS.Shared.DTOs.Cart;
using RetroVHS.Shared.DTOs.Rentals;
using System.Security.Claims;

namespace RetroVHS.Api.Controllers;

/// <summary>
/// Hanterar endpoints för varukorgen.
/// Alla endpoints kräver att användaren är inloggad (Authorize).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// Hämtar användar id från JWT-token.
    /// Varje inloggad användare har ett unikt id i sin token.
    /// </summary>
    private int GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Användaren saknar id i token.");

        return int.Parse(userIdClaim);
    }

    /// <summary>
    /// Hämtar den inloggade användarens aktiva varukorg.
    /// GET /api/cart
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var userId = GetUserId();
        var cart = await _cartService.GetCartAsync(userId);
        return Ok(cart);
    }
}