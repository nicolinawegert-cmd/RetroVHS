using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using RetroVHS.Shared.DTOs.Auth;

namespace RetroVHS.Client.Services;

/// <summary>
/// HTTP-klient som anropar API:ts auth-endpoints.
/// JWT-state delegeras till JwtAuthStateProvider.
/// </summary>
public class AuthClient : IAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly JwtAuthStateProvider _authStateProvider;
    private string? _refreshToken;

    public AuthClient(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _authStateProvider = (JwtAuthStateProvider)authStateProvider;
    }

    public async Task<AuthResultDto> LoginAsync(LoginRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        var result = await response.Content.ReadFromJsonAsync<AuthResultDto>();

        if (result is { Succeeded: true, Data: not null })
        {
            _refreshToken = result.Data.RefreshToken;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", result.Data.Token);
            _authStateProvider.MarkUserAsAuthenticated(result.Data.Token, result.Data.User);
        }

        return result!;
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
            catch
            {
                // Ignorera eventuella fel vid utloggning
            }
        }

        _refreshToken = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
        _authStateProvider.MarkUserAsLoggedOut();
    }
}
