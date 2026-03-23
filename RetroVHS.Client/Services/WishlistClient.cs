using System.Net.Http.Json;
using RetroVHS.Shared.DTOs.Wishlist;

namespace RetroVHS.Client.Services;

/// <summary>
/// HTTP-klient som anropar API:ts önskeliste-endpoints.
/// </summary>
public class WishlistClient : IWishlistClient
{
    private readonly HttpClient _httpClient;

    public WishlistClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

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
