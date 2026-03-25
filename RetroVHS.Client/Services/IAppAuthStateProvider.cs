using RetroVHS.Shared.DTOs.Auth;

namespace RetroVHS.Client.Services;

/// <summary>
/// Utökar Blazors AuthenticationStateProvider med appspecifika metoder
/// för att sätta och rensa auth-state utan att koppla beroenden till den
/// konkreta implementationen JwtAuthStateProvider.
/// </summary>
public interface IAppAuthStateProvider
{
    void MarkUserAsAuthenticated(string jwtToken, UserDto user);
    void MarkUserAsLoggedOut();
    void UpdateDisplayClaims(UserDto user);
}
