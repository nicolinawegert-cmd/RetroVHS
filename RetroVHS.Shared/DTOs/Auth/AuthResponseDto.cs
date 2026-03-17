namespace RetroVHS.Shared.DTOs.Auth;

/// <summary>
/// DTO som returneras efter lyckad inloggning eller registrering.
/// Innehåller JWT-token samt grundläggande info om användaren.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT-token som används för att autentisera framtida requests
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// När tokenen går ut (bra för frontend att veta)
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Information om användaren som är inloggad
    /// </summary>
    public UserDto User { get; set; } = null!;

    /// <summary>
    /// Användarens roller (t.ex. Admin, Member)
    /// </summary>
    public List<string> Roles { get; set; } = new();
}