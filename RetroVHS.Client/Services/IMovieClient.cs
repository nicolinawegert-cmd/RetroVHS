using RetroVHS.Shared.DTOs.Movies;

namespace RetroVHS.Client.Services;

/// <summary>
/// Klient för filmkatalogen — sökning, filtrering och hämtning av filmdata.
/// </summary>
public interface IMovieClient
{
    Task<List<MovieListDto>> SearchMoviesAsync(string searchTerm);
    Task<List<MovieListDto>> GetFeaturedMoviesAsync();
    Task<List<MovieListDto>> GetAllMoviesAsync();
    Task<List<MovieListDto>> GetTopRatedAsync();
    Task<List<MovieListDto>> GetBestsellersAsync();
    Task<List<GenreSectionDto>> GetTopGenreSectionsAsync();
    Task<MovieDetailsDto?> GetMovieDetailsAsync(int id);
}
