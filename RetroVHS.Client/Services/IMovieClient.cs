using RetroVHS.Shared.DTOs.Movies;

namespace RetroVHS.Client.Services;

/// <summary>
/// Klient-sidan av filmsökning — anropar API:t via HTTP.
/// </summary>
public interface IMovieClient
{
    Task<List<MovieListDto>> SearchMoviesAsync(string searchTerm);
}
