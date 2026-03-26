using RetroVHS.Shared.DTOs.Auth;

namespace RetroVHS.Client.Services;

/// <summary>
/// Kontrakt för autentisering mot API:t.
/// JWT lagras i minnet, refresh token krypterat i ProtectedLocalStorage.
/// </summary>
public interface IAuthClient
{
    /// <summary>POST api/auth/login — loggar in och sparar JWT + refresh token.</summary>
    Task<AuthResultDto> LoginAsync(LoginRequestDto request);

    /// <summary>POST api/auth/register — skapar konto och loggar in direkt.</summary>
    Task<AuthResultDto> RegisterAsync(RegisterRequestDto request);

    /// <summary>
    /// POST api/auth/logout — återkallar refresh token på servern,
    /// rensar JWT från minnet och tar bort refresh token från localStorage.
    /// </summary>
    Task LogoutAsync();

    /// <summary>
    /// Anropas vid sidladdning. Läser refresh token från localStorage och byter
    /// mot ett nytt JWT via POST api/auth/refresh — håller användaren inloggad i 7 dagar.
    /// </summary>
    Task TryRestoreSessionAsync();
}
