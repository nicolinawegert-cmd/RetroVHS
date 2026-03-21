using RetroVHS.Shared.DTOs.Movies;

namespace RetroVHS.Api.Services.Movies;

/// <summary>
/// Interface för MovieService som hanterar affärslogik relaterad till filmer.
/// </summary>

public interface IMovieService
{
  Task<List<MovieListDto>> GetMoviesAsync(MovieFilterDto filter);
  Task<MovieDetailsDto?> GetMovieByIdAsync(int id);
  Task<MovieDetailsDto> CreateMovieAsync(CreateMovieDto dto);
  Task<MovieDetailsDto?> UpdateMovieAsync(int id, UpdateMovieDto dto);
}