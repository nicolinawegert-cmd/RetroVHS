using System.Net.Http.Json;
using RetroVHS.Shared.DTOs.Cart;
using RetroVHS.Shared.DTOs.Rentals;

namespace RetroVHS.Client.Services;

/// <summary>
/// HTTP-klient som kommunicerar med varukorgs-API:t.
/// Varje metod gör ett HTTP-anrop mot CartController.
/// </summary>
public class CartClient : ICartClient
{
    private readonly HttpClient _httpClient;

    public CartClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<CartDto?> GetCartAsync()
    {
        try
        {
            // Försök hämta varukorgen från API:t. Om det inte går (t.ex. ingen varukorg eller nätverksfel), returnera null.
            return await _httpClient.GetFromJsonAsync<CartDto>("api/cart");
        }
        catch
        {
            return null;
        }
    }


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