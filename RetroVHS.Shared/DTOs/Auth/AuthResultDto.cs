namespace RetroVHS.Shared.DTOs.Auth;

/// <summary>
/// Resultatobjekt som används internt mellan controller och service
/// för att beskriva om en auth-operation lyckades eller inte.
/// </summary>
public class AuthResultDto
{
    /// <summary>
    /// Anger om operationen lyckades
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Ett meddelande som kan skickas tillbaka till klienten
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Eventuella valideringsfel eller andra felmeddelanden
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Auth-svar med token och användardata om operationen lyckades
    /// </summary>
    public AuthResponseDto? Data { get; set; }
}