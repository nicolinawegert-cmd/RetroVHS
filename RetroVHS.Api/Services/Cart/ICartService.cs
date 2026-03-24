using RetroVHS.Shared.DTOs.Cart;
using RetroVHS.Shared.DTOs.Rentals;

namespace RetroVHS.Api.Services.Cart;

/// <summary>
/// Kontrakt för varukorgshantering.
/// Definierar vilka operationer som kan utföras på en varukorg.
/// </summary>
public interface ICartService
{
    /// <summary>
    /// Hämtar den aktiva varukorgen för en användare.
    /// Om ingen aktiv varukorg finns skapas en ny automatiskt.
    /// </summary>
    Task<CartDto> GetCartAsync(int userId);

    /// <summary>
    /// Lägger till en film i användarens varukorg.
    /// </summary>
    Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto);

    /// <summary>
    /// Tar bort en specifik film ur varukorgen.
    /// Returnerar true om borttagningen lyckades.
    /// </summary>
    Task<bool> RemoveFromCartAsync(int userId, int cartItemId);

    /// <summary>
    /// Genomför checkout: skapar uthyrningar (Rental) för varje film
    /// i varukorgen och markerar varukorgen som utcheckad.
    /// </summary>
    Task<CheckoutResponseDto> CheckoutAsync(int userId, CheckoutDto dto);
}