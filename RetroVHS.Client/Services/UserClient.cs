using System.Net.Http.Json;
using System.Text.Json;
using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Client.Services;

/// <summary>
/// HTTP-klient som anropar API:ts användarendpoints.
/// </summary>
public class UserClient : IUserClient
{
    private readonly HttpClient _httpClient;

    public UserClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

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

            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            var message = body.TryGetProperty("message", out var msg) ? msg.GetString() : null;
            return (null, message ?? "Kunde inte uppdatera profilen.");
        }
        catch
        {
            return (null, "Kunde inte uppdatera profilen.");
        }
    }

    public async Task<string?> ChangePasswordAsync(ChangePasswordDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("api/users/me/password", dto);
            if (response.IsSuccessStatusCode)
                return null;

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
}
