using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.Auth;

/// <summary>
/// DTO som används vid inloggning.
/// Innehåller endast den information som behövs för att autentisera användaren.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// Användarens e-postadress
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Användarens lösenord
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}