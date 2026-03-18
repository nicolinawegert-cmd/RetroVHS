using RetroVHS.Api.Models;

namespace RetroVHS.Api.Services.Auth;

/// <summary>
/// Ansvarar för att skapa JWT-tokens för autentisering.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Skapar en JWT-token för en användare
    /// </summary>
    /// <param name="user">Användaren</param>
    /// <param name="roles">Användarens roller</param>
    /// <returns>JWT-token som string</returns>
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
}