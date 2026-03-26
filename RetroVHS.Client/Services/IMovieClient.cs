using RetroVHS.Shared.DTOs.Movies;

namespace RetroVHS.Client.Services;

/// <summary>
/// Kontrakt för filmkatalogen — sökning, filtrering och hämtning av filmdata.
/// Alla metoder är publika (kräver ingen inloggning).
/// </summary>
public interface IMovieClient
{
    /// <summary>GET api/movies?searchTerm=... — söker filmer på titel (används i Header-sökfältet).</summary>
    Task<List<MovieListDto>> SearchMoviesAsync(string searchTerm);

    /// <summary>GET api/movies?featured=true — hämtar utvalda filmer.</summary>
    Task<List<MovieListDto>> GetFeaturedMoviesAsync();

    /// <summary>GET api/movies — hämtar alla filmer (används på /movies-sidan).</summary>
    Task<List<MovieListDto>> GetAllMoviesAsync();

    /// <summary>GET api/movies/top-rated — topp 5 filmer efter genomsnittsbetyg, alltid exakt 5 (fylls med defaults).</summary>
    Task<List<MovieListDto>> GetTopRatedAsync();

    /// <summary>GET api/movies/bestsellers — topp 5 filmer efter antal uthyrningar, alltid exakt 5.</summary>
    Task<List<MovieListDto>> GetBestsellersAsync();

    /// <summary>GET api/movies/genre-sections — topp 5 mest populära genres, topp 5 filmer per genre.</summary>
    Task<List<GenreSectionDto>> GetTopGenreSectionsAsync();

    /// <summary>GET api/movies/{id} — fullständig filmdata inkl. recensioner, skådespelare och trailer.</summary>
    Task<MovieDetailsDto?> GetMovieDetailsAsync(int id);
}
