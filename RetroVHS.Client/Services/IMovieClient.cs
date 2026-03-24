using RetroVHS.Shared.DTOs.Movies;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Client.Services;

/// <summary>
/// Klient-sidan av filmsökning — anropar API:t via HTTP.
/// </summary>
public interface IMovieClient
{
    Task<List<MovieListDto>> SearchMoviesAsync(string searchTerm);
    Task<List<MovieListDto>> GetFeaturedMoviesAsync();
    Task<List<MovieListDto>> GetAllMoviesAsync();
    Task<MovieDetailsDto?> GetMovieDetailsAsync(int id);
    Task<ReviewDto?> CreateReviewAsync(CreateReviewDto dto);
    Task<ReviewDto?> UpdateReviewAsync(UpdateReviewDto dto);
}
