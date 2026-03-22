using System.Net.Http.Json;
using RetroVHS.Shared.DTOs.Movies;

namespace RetroVHS.Client.Services;

/// <summary>
/// HTTP-klient som söker filmer via API:t.
/// </summary>
public class MovieClient : IMovieClient
{
    private readonly HttpClient _httpClient;

    public MovieClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MovieListDto>> SearchMoviesAsync(string searchTerm)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<MovieListDto>>(
                $"api/movies?searchTerm={Uri.EscapeDataString(searchTerm)}");
            return result ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<List<MovieListDto>> GetFeaturedMoviesAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<MovieListDto>>(
                "api/movies?featured=true");
            return result ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<List<MovieListDto>> GetAllMoviesAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<MovieListDto>>("api/movies");
            return result ?? [];
        }
        catch
        {
            return [];
        }
    }
}
