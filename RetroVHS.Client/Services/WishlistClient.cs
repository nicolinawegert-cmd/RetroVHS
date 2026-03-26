using System.Net.Http.Json;
using RetroVHS.Shared.DTOs.Wishlist;

namespace RetroVHS.Client.Services;

/// <summary>
/// HTTP-klient som anropar API:ts önskeliste-endpoints.
/// Alla anrop kräver inloggning — JWT skickas automatiskt via Authorization-headern.
/// </summary>
public class WishlistClient : IWishlistClient
{
    private readonly HttpClient _httpClient;

    public WishlistClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // GET api/wishlist — hämtar alla önskelisteobjekt för inloggad användare
    public async Task<List<WishlistItemDto>> GetWishlistAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<WishlistItemDto>>("api/wishlist");
            return result ?? [];
        }
        catch
        {
            return [];
        }
    }

    // POST api/wishlist — lägger till film i önskelistan
    public async Task<bool> AddToWishlistAsync(int movieId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/wishlist", new AddToWishlistDto { MovieId = movieId });
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // DELETE api/wishlist/{movieId} — tar bort film från önskelistan (notera: movieId, inte wishlistItemId)
    public async Task<bool> RemoveFromWishlistAsync(int movieId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/wishlist/{movieId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
