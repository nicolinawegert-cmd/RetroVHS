using RetroVHS.Shared.DTOs.Admin;
using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Genres;
using RetroVHS.Shared.DTOs.Movies;
using RetroVHS.Shared.DTOs.Persons;
using RetroVHS.Shared.DTOs.ProductionCompanies;
using RetroVHS.Shared.DTOs.Rentals;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Client.Services;

/// <summary>
/// Kontrakt för alla admin-operationer. Implementeras av AdminClient
/// som anropar skyddade endpoints under /api/admin/* (kräver Admin-roll).
/// </summary>
public interface IAdminClient
{
    // ── Dashboard ──────────────────────────────────────────────────

    /// <summary>GET api/admin/stats — hämtar aggregerad statistik för dashboard.</summary>
    Task<AdminDashboardDto?> GetDashboardStatsAsync();

    // ── Users ──────────────────────────────────────────────────────

    /// <summary>GET api/admin/users — lista alla användare.</summary>
    Task<List<UserDto>> GetAllUsersAsync();

    /// <summary>GET api/admin/users/{id} — hämta en specifik användare.</summary>
    Task<UserDto?> GetUserByIdAsync(int id);

    /// <summary>DELETE api/admin/users/{id} — radera användare och all tillhörande data.</summary>
    Task<bool> DeleteUserAsync(int id);

    /// <summary>PUT api/admin/users/{id}/block — blockera en användare (förhindrar inloggning).</summary>
    Task<bool> BlockUserAsync(int id);

    /// <summary>PUT api/admin/users/{id}/unblock — häver blockering.</summary>
    Task<bool> UnblockUserAsync(int id);

    /// <summary>PUT api/admin/users/{id}/nickname — sätter nickname till null (moderation).</summary>
    Task<bool> SetNicknameNullAsync(int id);

    // ── Reviews ────────────────────────────────────────────────────

    /// <summary>GET api/admin/users/{userId}/reviews — hämtar alla recensioner för en användare.</summary>
    Task<List<ReviewDto>> GetUserReviewsAsync(int userId);

    /// <summary>DELETE api/admin/reviews/{reviewId}/comment — tar bort kommentaren men behåller betyget.</summary>
    Task<bool> RemoveReviewCommentAsync(int reviewId);

    /// <summary>DELETE api/admin/reviews/{reviewId} — raderar hela recensionen.</summary>
    Task<bool> DeleteReviewAsync(int reviewId);

    // ── Rentals ────────────────────────────────────────────────────

    /// <summary>GET api/admin/users/{userId}/rentals — hämtar alla beställningar för en användare.</summary>
    Task<List<RentalDto>> GetUserRentalsAsync(int userId);

    /// <summary>PUT api/admin/rentals/{rentalId}/cancel — avbryter en aktiv beställning.</summary>
    Task<bool> CancelRentalAsync(int rentalId);

    // ── Movies ─────────────────────────────────────────────────────

    /// <summary>GET api/admin/movies — hämtar alla filmer (admin-vy med fler detaljer).</summary>
    Task<List<MovieListDto>> GetAllMoviesAsync();

    /// <summary>GET api/admin/movies/{id} — hämtar fullständig filmdata för redigering.</summary>
    Task<MovieDetailsDto?> GetMovieByIdAsync(int id);

    /// <summary>POST api/admin/movies — skapar en ny film.</summary>
    Task<MovieDetailsDto?> CreateMovieAsync(CreateMovieDto dto);

    /// <summary>PUT api/admin/movies/{id} — uppdaterar en befintlig film.</summary>
    Task<MovieDetailsDto?> UpdateMovieAsync(int id, UpdateMovieDto dto);

    /// <summary>DELETE api/admin/movies/{id} — raderar en film permanent.</summary>
    Task<bool> DeleteMovieAsync(int id);

    // ── Reference data ─────────────────────────────────────────────

    /// <summary>GET api/genres — hämtar alla genres (används i MovieForm).</summary>
    Task<List<GenreDto>> GetGenresAsync();

    /// <summary>POST api/genres — skapar en ny genre direkt från MovieForm.</summary>
    Task<GenreDto?> CreateGenreAsync(string name);

    /// <summary>GET api/persons?search=... — söker efter personer (regissörer/skådespelare) med debounce.</summary>
    Task<List<PersonDto>> GetPersonsAsync(string? search = null);

    /// <summary>GET api/production-companies — hämtar alla produktionsbolag.</summary>
    Task<List<ProductionCompanyDto>> GetProductionCompaniesAsync();

    /// <summary>POST api/production-companies — skapar ett nytt produktionsbolag direkt från MovieForm.</summary>
    Task<ProductionCompanyDto?> CreateProductionCompanyAsync(string name);
}
