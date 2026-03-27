using System.Net.Http.Json;
using System.Text.Json;
using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Rentals;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Client.Services;

/// <summary>
/// HTTP-klient som anropar API:ts användarendpoints.
/// Alla anrop kräver inloggning — JWT skickas automatiskt via Authorization-headern.
/// </summary>
public class UserClient : IUserClient
{
    private readonly HttpClient _httpClient;

    public UserClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // GET api/users/me — hämtar profildata för den inloggade användaren
    public async Task<UserDto?> GetCurrentUserAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserDto>("api/users/me");
        }
        catch
        {
            return null;
        }
    }

    // PUT api/users/me — uppdaterar profil. Vid fel läses felmeddelandet ur JSON-svaret
    // och returneras som en tuple (null, felmeddelande) istället för att kasta exception.
    public async Task<(UserDto? User, string? Error)> UpdateProfileAsync(UpdateUserProfileDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("api/users/me", dto);
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();
                return (user, null);
            }

            // Plocka ut felmeddelandet från API-svaret om det finns
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            var message = body.TryGetProperty("message", out var msg) ? msg.GetString() : null;
            return (null, message ?? "Kunde inte uppdatera profilen.");
        }
        catch
        {
            return (null, "Kunde inte uppdatera profilen.");
        }
    }

    // PUT api/users/me/password — returnerar null vid framgång, felmeddelande vid misslyckande.
    // API:t kan returnera en lista av fel (t.ex. lösenordskrav) som sammanfogas till en sträng.
    public async Task<string?> ChangePasswordAsync(ChangePasswordDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("api/users/me/password", dto);
            if (response.IsSuccessStatusCode)
                return null;

            // API kan returnera en "errors"-array med flera valideringsfel
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (body.TryGetProperty("errors", out var errorsEl))
            {
                var errors = errorsEl.EnumerateArray()
                    .Select(e => e.GetString() ?? string.Empty)
                    .Where(e => !string.IsNullOrEmpty(e))
                    .ToList();
                if (errors.Count > 0)
                    return string.Join(" ", errors);
            }

            return "Kunde inte byta lösenord.";
        }
        catch
        {
            return "Kunde inte byta lösenord.";
        }
    }

    // GET api/users/me/reviews
    public async Task<List<ReviewDto>> GetMyReviewsAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<ReviewDto>>("api/users/me/reviews");
            return result ?? [];
        }
        catch
        {
            return [];
        }
    }

    // GET api/users/me/rentals
    public async Task<List<RentalDto>> GetMyOrdersAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<RentalDto>>("api/users/me/rentals");
            return result ?? [];
        }
        catch
        {
            return [];
        }
    }

    // PUT api/rentals/{rentalId}/complete — null body (ingen request body behövs)
    public async Task<bool> CompleteRentalAsync(int rentalId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/rentals/{rentalId}/complete", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // PUT api/rentals/{rentalId}/cancel — null body
    public async Task<bool> CancelRentalAsync(int rentalId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/rentals/{rentalId}/cancel", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}
