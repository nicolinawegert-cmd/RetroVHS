using RetroVHS.Shared.DTOs.Movies;

namespace RetroVHS.Api.Services.Movies;

/// <summary>
/// Interface för MovieService som hanterar affärslogik relaterad till filmer.
/// </summary>

public interface IMovieService
{
  /// <summary>
  /// Hämtar en lista med filmer baserat på angivna filter och sorteringsval.
  /// </summary>
  Task<List<MovieListDto>> GetMoviesAsync(MovieFilterDto filter);

  /// <summary>
  /// Hämtar detaljerad information om en specifik film.
  /// </summary>
  Task<MovieDetailsDto?> GetMovieByIdAsync(int id);

  /// <summary>
  /// Skapar en ny film och returnerar den sparade detaljrepresentationen.
  /// </summary>
  Task<MovieDetailsDto> CreateMovieAsync(CreateMovieDto dto);

  /// <summary>
  /// Uppdaterar en befintlig film och returnerar den uppdaterade detaljrepresentationen.
  /// </summary>
  Task<MovieDetailsDto?> UpdateMovieAsync(int id, UpdateMovieDto dto);

  /// <summary>
  /// Tar bort en film från katalogen.
  /// </summary>
  Task<bool> DeleteMovieAsync(int id);

  /// <summary>
  /// Hämtar de fem filmer med högst genomsnittsbetyg.
  /// </summary>
  Task<List<MovieListDto>> GetTopRatedAsync();

  /// <summary>
  /// Hämtar de fem filmer som hyrts flest gånger.
  /// </summary>
  Task<List<MovieListDto>> GetBestsellersAsync();
}
