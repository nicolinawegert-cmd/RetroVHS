using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.Auth;

/// <summary>
/// DTO som används när klienten vill byta ut en refresh token
/// mot en ny access token.
/// </summary>
public class RefreshTokenRequestDto
{
    /// <summary>
    /// Refresh token som klienten skickar in
    /// </summary>
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}