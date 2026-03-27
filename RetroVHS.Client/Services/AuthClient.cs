using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RetroVHS.Shared.DTOs.Auth;

namespace RetroVHS.Client.Services;

/// <summary>
/// HTTP-klient som anropar API:ts auth-endpoints.
/// JWT-state delegeras till IAppAuthStateProvider.
/// Refresh token sparas krypterat i localStorage för att överleva sidladdning.
/// </summary>
public class AuthClient : IAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly IAppAuthStateProvider _authStateProvider;
    private readonly ProtectedLocalStorage _storage;
    private string? _refreshToken;

    private const string StorageKey = "retrovhs_refresh";

    public AuthClient(HttpClient httpClient, IAppAuthStateProvider authStateProvider,
        ProtectedLocalStorage storage)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _storage = storage;
    }

    public async Task<AuthResultDto> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            var result = await response.Content.ReadFromJsonAsync<AuthResultDto>();
            if (result is null)
                return new AuthResultDto { Succeeded = false, Message = "Ogiltigt svar från servern." };

            if (result is { Succeeded: true, Data: not null })
                await SetAuthStateAsync(result.Data);

            return result;
        }
        catch
        {
            return new AuthResultDto { Succeeded = false, Message = "Kunde inte nå servern. Försök igen." };
        }
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            var result = await response.Content.ReadFromJsonAsync<AuthResultDto>();
            if (result is null)
                return new AuthResultDto { Succeeded = false, Message = "Ogiltigt svar från servern." };

            if (result is { Succeeded: true, Data: not null })
                await SetAuthStateAsync(result.Data);

            return result;
        }
        catch
        {
            return new AuthResultDto { Succeeded = false, Message = "Kunde inte nå servern. Försök igen." };
        }
    }

    public async Task LogoutAsync()
    {
        if (_refreshToken is not null)
        {
            try
            {
                await _httpClient.PostAsJsonAsync("api/auth/logout",
                    new RefreshTokenRequestDto { RefreshToken = _refreshToken });
            }
            catch { }
        }

        _refreshToken = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
        _authStateProvider.MarkUserAsLoggedOut();

        try { await _storage.DeleteAsync(StorageKey); } catch { }
    }

    public async Task TryRestoreSessionAsync()
    {
        try
        {
            var stored = await _storage.GetAsync<string>(StorageKey);
            if (!stored.Success || string.IsNullOrEmpty(stored.Value))
                return;

            var response = await _httpClient.PostAsJsonAsync("api/auth/refresh",
                new RefreshTokenRequestDto { RefreshToken = stored.Value });

            if (!response.IsSuccessStatusCode)
            {
                await _storage.DeleteAsync(StorageKey);
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResultDto>();
            if (result is { Succeeded: true, Data: not null })
                await SetAuthStateAsync(result.Data);
            else
                await _storage.DeleteAsync(StorageKey);
        }
        catch { }
    }

    private async Task SetAuthStateAsync(AuthResponseDto data)
    {
        _refreshToken = data.RefreshToken;
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", data.Token);
        _authStateProvider.MarkUserAsAuthenticated(data.Token, data.User);

        try { await _storage.SetAsync(StorageKey, data.RefreshToken); } catch { }
    }
}
