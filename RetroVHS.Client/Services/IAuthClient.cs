using RetroVHS.Shared.DTOs.Auth;

namespace RetroVHS.Client.Services;

/// <summary>
/// HTTP-klient för autentisering mot API:t.
/// JWT-state hanteras av JwtAuthStateProvider.
/// </summary>
public interface IAuthClient
{
    Task<AuthResultDto> LoginAsync(LoginRequestDto request);
    Task<AuthResultDto> RegisterAsync(RegisterRequestDto request);
    Task LogoutAsync();
}
