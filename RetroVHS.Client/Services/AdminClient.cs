using System.Net.Http.Json;
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
/// HTTP-klient för admin-operationer.
/// Alla anrop kräver Admin-roll — JWT-tokenen med rätt roll skickas automatiskt
/// via Authorization-headern som satts av AuthClient vid inloggning.
/// </summary>
public class AdminClient : IAdminClient
{
    private readonly HttpClient _httpClient;
    public AdminClient(HttpClient httpClient) => _httpClient = httpClient;

    // ── Dashboard ──────────────────────────────────────────────────
    public async Task<AdminDashboardDto?> GetDashboardStatsAsync()
    {
        try { return await _httpClient.GetFromJsonAsync<AdminDashboardDto>("api/admin/stats"); }
        catch { return null; }
    }

    // ── Users ──────────────────────────────────────────────────────
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        try { return await _httpClient.GetFromJsonAsync<List<UserDto>>("api/admin/users") ?? []; }
        catch { return []; }
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        try { return await _httpClient.GetFromJsonAsync<UserDto>($"api/admin/users/{id}"); }
        catch { return null; }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        try { return (await _httpClient.DeleteAsync($"api/admin/users/{id}")).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> BlockUserAsync(int id)
    {
        try { return (await _httpClient.PutAsync($"api/admin/users/{id}/block", null)).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> UnblockUserAsync(int id)
    {
        try { return (await _httpClient.PutAsync($"api/admin/users/{id}/unblock", null)).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> SetNicknameNullAsync(int id)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/admin/users/{id}/nickname", new AdminSetNicknameDto { Nickname = null });
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // ── Reviews ────────────────────────────────────────────────────
    public async Task<List<ReviewDto>> GetUserReviewsAsync(int userId)
    {
        try { return await _httpClient.GetFromJsonAsync<List<ReviewDto>>($"api/admin/users/{userId}/reviews") ?? []; }
        catch { return []; }
    }

    public async Task<bool> RemoveReviewCommentAsync(int reviewId)
    {
        try { return (await _httpClient.DeleteAsync($"api/admin/reviews/{reviewId}/comment")).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> DeleteReviewAsync(int reviewId)
    {
        try { return (await _httpClient.DeleteAsync($"api/admin/reviews/{reviewId}")).IsSuccessStatusCode; }
        catch { return false; }
    }

    // ── Rentals ────────────────────────────────────────────────────
    public async Task<List<RentalDto>> GetUserRentalsAsync(int userId)
    {
        try { return await _httpClient.GetFromJsonAsync<List<RentalDto>>($"api/admin/users/{userId}/rentals") ?? []; }
        catch { return []; }
    }

    public async Task<bool> CancelRentalAsync(int rentalId)
    {
        try { return (await _httpClient.PutAsync($"api/admin/rentals/{rentalId}/cancel", null)).IsSuccessStatusCode; }
        catch { return false; }
    }

    // ── Movies ─────────────────────────────────────────────────────
    public async Task<List<MovieListDto>> GetAllMoviesAsync()
    {
        try { return await _httpClient.GetFromJsonAsync<List<MovieListDto>>("api/admin/movies") ?? []; }
        catch { return []; }
    }

    public async Task<MovieDetailsDto?> GetMovieByIdAsync(int id)
    {
        try { return await _httpClient.GetFromJsonAsync<MovieDetailsDto>($"api/admin/movies/{id}"); }
        catch { return null; }
    }

    public async Task<MovieDetailsDto?> CreateMovieAsync(CreateMovieDto dto)
    {
        try
        {
            var r = await _httpClient.PostAsJsonAsync("api/admin/movies", dto);
            return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<MovieDetailsDto>() : null;
        }
        catch { return null; }
    }

    public async Task<MovieDetailsDto?> UpdateMovieAsync(int id, UpdateMovieDto dto)
    {
        try
        {
            var r = await _httpClient.PutAsJsonAsync($"api/admin/movies/{id}", dto);
            return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<MovieDetailsDto>() : null;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteMovieAsync(int id)
    {
        try { return (await _httpClient.DeleteAsync($"api/admin/movies/{id}")).IsSuccessStatusCode; }
        catch { return false; }
    }

    // ── Reference data ─────────────────────────────────────────────
    public async Task<List<GenreDto>> GetGenresAsync()
    {
        try { return await _httpClient.GetFromJsonAsync<List<GenreDto>>("api/genres") ?? []; }
        catch { return []; }
    }

    public async Task<List<PersonDto>> GetPersonsAsync(string? search = null)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(search) ? "api/persons" : $"api/persons?search={Uri.EscapeDataString(search)}";
            return await _httpClient.GetFromJsonAsync<List<PersonDto>>(url) ?? [];
        }
        catch { return []; }
    }

    public async Task<List<ProductionCompanyDto>> GetProductionCompaniesAsync()
    {
        try { return await _httpClient.GetFromJsonAsync<List<ProductionCompanyDto>>("api/production-companies") ?? []; }
        catch { return []; }
    }

    public async Task<GenreDto?> CreateGenreAsync(string name)
    {
        try
        {
            var r = await _httpClient.PostAsJsonAsync("api/genres", new CreateGenreDto { Name = name });
            return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<GenreDto>() : null;
        }
        catch { return null; }
    }

    public async Task<ProductionCompanyDto?> CreateProductionCompanyAsync(string name)
    {
        try
        {
            var r = await _httpClient.PostAsJsonAsync("api/production-companies", new CreateProductionCompanyDto { Name = name });
            return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<ProductionCompanyDto>() : null;
        }
        catch { return null; }
    }
}
