using RetroVHS.Shared.DTOs.Movies;

namespace RetroVHS.Api.Services.Movies;

public interface IMovieService
{
  Task<List<MovieListDto>> GetMoviesAsync(MovieFilterDto filter);
  Task<MovieDetailsDto?> GetMovieByIdAsync(int id);
  Task<MovieDetailsDto> CreateMovieAsync(CreateMovieDto dto);
  Task<MovieDetailsDto?> UpdateMovieAsync(int id, UpdateMovieDto dto);
  Task<bool> DeleteMovieAsync(int id);
  Task<List<MovieListDto>> GetTopRatedAsync();
  Task<List<MovieListDto>> GetBestsellersAsync();
  Task<List<GenreSectionDto>> GetTopGenreSectionsAsync();
}
