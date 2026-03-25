using System.Net.Http.Json;
using RetroVHS.Shared.DTOs.Movies;
using RetroVHS.Shared.DTOs.Reviews;

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

    public async Task<List<MovieListDto>> GetTopRatedAsync()
    {
        try { return await _httpClient.GetFromJsonAsync<List<MovieListDto>>("api/movies/top-rated") ?? []; }
        catch { return []; }
    }

    public async Task<List<MovieListDto>> GetBestsellersAsync()
    {
        try { return await _httpClient.GetFromJsonAsync<List<MovieListDto>>("api/movies/bestsellers") ?? []; }
        catch { return []; }
    }

    public async Task<List<GenreSectionDto>> GetTopGenreSectionsAsync()
    {
        try { return await _httpClient.GetFromJsonAsync<List<GenreSectionDto>>("api/movies/genre-sections") ?? []; }
        catch { return []; }
    }

    public async Task<MovieDetailsDto?> GetMovieDetailsAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<MovieDetailsDto>($"api/movies/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReviewDto?> CreateReviewAsync(CreateReviewDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/reviews", dto);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ReviewDto>();
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReviewDto?> UpdateReviewAsync(UpdateReviewDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/reviews/{dto.Id}", dto);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ReviewDto>();
            return null;
        }
        catch
        {
            return null;
        }
    }
}
