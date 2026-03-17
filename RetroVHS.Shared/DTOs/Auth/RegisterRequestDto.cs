using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.Auth;

/// <summary>
/// DTO som används när en ny användare registrerar sig.
/// Vi skickar bara den data som backend behöver för att skapa kontot.
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// Användarens förnamn
    /// </summary>
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Användarens efternamn
    /// </summary>
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Valfritt nickname som kan visas i recensioner och kommentarer
    /// </summary>
    [StringLength(50)]
    public string? Nickname { get; set; }

    /// <summary>
    /// E-postadress som används för inloggning
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Lösenord för kontot
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Bekräftelse av lösenord för att minska risken för felskrivning
    /// </summary>
    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}