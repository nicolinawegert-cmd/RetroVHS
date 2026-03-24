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

    /// <summary>
    /// Lägger till en film i användarens varukorg.
    /// POST /api/cart
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CartDto>> AddToCart([FromBody] AddToCartDto dto)
    {
        try
        {
            var userId = GetUserId();
            var cart = await _cartService.AddToCartAsync(userId, dto);
            return Ok(cart);
        }
        catch (InvalidOperationException ex)
        {
            // T.ex. "Filmen finns redan" eller "Slut i lager"
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            // T.ex. "Filmen hittades inte"
            return NotFound(new { message = ex.Message });
        }
    }


    /// <summary>
    /// Tar bort en film ur användarens varukorg.
    /// DELETE /api/cart/{cartItemId}
    /// </summary>
    [HttpDelete("{cartItemId:int}")]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        var userId = GetUserId();
        var removed = await _cartService.RemoveFromCartAsync(userId, cartItemId);

        if (!removed)
            return NotFound(new { message = "Raden hittades inte i varukorgen." });

        return NoContent();
    }

    /// <summary>
    /// Genomför checkout – skapar uthyrningar för alla filmer i varukorgen.
    /// POST /api/cart/checkout
    /// </summary>
    [HttpPost("checkout")]
    public async Task<ActionResult<CheckoutResponseDto>> Checkout([FromBody] CheckoutDto dto)
    {
        var userId = GetUserId();
        var result = await _cartService.CheckoutAsync(userId, dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}