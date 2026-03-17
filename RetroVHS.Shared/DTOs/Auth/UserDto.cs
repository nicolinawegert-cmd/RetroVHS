namespace RetroVHS.Shared.DTOs.Auth;

/// <summary>
/// DTO som representerar den användarinformation vi vill skicka tillbaka till klienten.
/// Vi skickar inte hela ApplicationUser eftersom backend-modellen innehåller
/// fler interna fält än vad frontend behöver.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Användarens id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Användarens förnamn
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Användarens efternamn
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Valfritt nickname
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// Användarens e-postadress
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Anger om användaren är blockerad
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Fullständigt namn, praktiskt för visning i frontend
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
}