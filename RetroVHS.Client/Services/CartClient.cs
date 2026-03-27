using System.Net.Http.Json;
using RetroVHS.Shared.DTOs.Cart;
using RetroVHS.Shared.DTOs.Rentals;

namespace RetroVHS.Client.Services;

/// <summary>
/// HTTP-klient som kommunicerar med varukorgs-API:t.
/// Alla anrop kräver inloggning — JWT skickas automatiskt via Authorization-headern.
/// </summary>
public class CartClient : ICartClient
{
    private readonly HttpClient _httpClient;

    public CartClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // GET api/cart — hämtar den aktiva varukorgen för inloggad användare
    public async Task<CartDto?> GetCartAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CartDto>("api/cart");
        }
        catch
        {
            return null;
        }
    }

    // POST api/cart — lägger till en film, returnerar uppdaterad varukorg (inkl. nytt TotalItems)
    public async Task<CartDto?> AddToCartAsync(int movieId)
    {
        try
        {
            var dto = new AddToCartDto { MovieId = movieId };
            var response = await _httpClient.PostAsJsonAsync("api/cart", dto);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<CartDto>();

            return null;
        }
        catch
        {
            return null;
        }
    }

    // DELETE api/cart/{cartItemId} — tar bort ett specifikt cart-item (inte movieId, utan cart-radens ID)
    public async Task<bool> RemoveFromCartAsync(int cartItemId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/cart/{cartItemId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // POST api/cart/checkout — genomför köpet och skapar uthyrningsposter i databasen
    public async Task<CheckoutResponseDto?> CheckoutAsync(int cartId)
    {
        try
        {
            var dto = new CheckoutDto { CartId = cartId };
            var response = await _httpClient.PostAsJsonAsync("api/cart/checkout", dto);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<CheckoutResponseDto>();

            return null;
        }
        catch
        {
            return null;
        }
    }
}