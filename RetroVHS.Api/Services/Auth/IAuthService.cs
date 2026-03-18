using RetroVHS.Shared.DTOs.Auth;

namespace RetroVHS.Api.Services.Auth;

/// <summary>
/// Hanterar all autentiseringslogik (register, login, refresh, logout)
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registrerar en ny användare
    /// </summary>
    Task<AuthResultDto> RegisterAsync(RegisterRequestDto request);

    /// <summary>
    /// Loggar in en användare
    /// </summary>
    Task<AuthResultDto> LoginAsync(LoginRequestDto request);

    /// <summary>
    /// Hämtar en ny access token med hjälp av refresh token
    /// </summary>
    Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto request);

    /// <summary>
    /// Loggar ut användaren (revoke refresh token)
    /// </summary>
    Task<AuthResultDto> LogoutAsync(string refreshToken);
}