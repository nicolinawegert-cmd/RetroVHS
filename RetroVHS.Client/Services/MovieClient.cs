using System.Net.Http.Json;
using RetroVHS.Shared.DTOs.Movies;

namespace RetroVHS.Client.Services;

/// <summary>
/// HTTP-klient som hämtar filmdata från API:t.
/// Alla endpoints är publika — kräver ingen inloggning.
/// Fel (nätverksfel, 404, etc.) hanteras tyst och returnerar tom lista eller null.
/// </summary>
public class MovieClient : IMovieClient
{
    private readonly HttpClient _httpClient;

    public MovieClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // GET api/movies?searchTerm=... — söktermen URL-enkodas för att hantera specialtecken
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

    // GET api/movies?featured=true
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

    // GET api/movies
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

    // GET api/movies/top-rated — returnerar alltid exakt 5 filmer (API fyller ut med defaults)
    public async Task<List<MovieListDto>> GetTopRatedAsync()
    {
        try { return await _httpClient.GetFromJsonAsync<List<MovieListDto>>("api/movies/top-rated") ?? []; }
        catch { return []; }
    }

    // GET api/movies/bestsellers — returnerar alltid exakt 5 filmer
    public async Task<List<MovieListDto>> GetBestsellersAsync()
    {
        try { return await _httpClient.GetFromJsonAsync<List<MovieListDto>>("api/movies/bestsellers") ?? []; }
        catch { return []; }
    }

    // GET api/movies/genre-sections — returnerar en lista av GenreSectionDto,
    // en per genre (topp 5 genres), varje med 5 filmer
    public async Task<List<GenreSectionDto>> GetTopGenreSectionsAsync()
    {
        try { return await _httpClient.GetFromJsonAsync<List<GenreSectionDto>>("api/movies/genre-sections") ?? []; }
        catch { return []; }
    }

    // GET api/movies/{id} — fullständig filmdata inkl. cast, reviews och trailer-URL
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

}
