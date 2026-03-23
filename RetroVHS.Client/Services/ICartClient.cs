using RetroVHS.Shared.DTOs.Cart;
using RetroVHS.Shared.DTOs.Rentals;

namespace RetroVHS.Client.Services;

/// <summary>
/// Kontrakt för klientens kommunikation med varukorgs-API:t.
/// Varje metod motsvarar en endpoint i CartController.
/// </summary>
public interface ICartClient
{
    /// <summary>
    /// Hämtar den aktiva varukorgen från API:t.
    /// </summary>
    Task<CartDto?> GetCartAsync();

    /// <summary>
    /// Lägger till en film i varukorgen.
    /// </summary>
    Task<CartDto?> AddToCartAsync(int movieId);

    /// <summary>
    /// Tar bort en film ur varukorgen.
    /// </summary>
    Task<bool> RemoveFromCartAsync(int cartItemId);

    /// <summary>
    /// Genomför köpet och returnerar en bekräftelse.
    /// </summary>
    Task<CheckoutResponseDto?> CheckoutAsync(int cartId);
}