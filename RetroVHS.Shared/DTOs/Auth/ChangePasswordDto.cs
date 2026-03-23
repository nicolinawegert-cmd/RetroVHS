using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.Auth;

/// <summary>
/// DTO som används när en användare vill byta sitt lösenord.
/// </summary>
public class ChangePasswordDto
{
  /// <summary>
  /// Användarens nuvarande lösenord.
  /// </summary>
  [Required]
  public string CurrentPassword { get; set; } = string.Empty;

  /// <summary>
  /// Det nya lösenordet.
  /// </summary>
  [Required]
  [StringLength(100, MinimumLength = 6, ErrorMessage = "Lösenordet måste vara minst 6 tecken.")]
  [RegularExpression(@"^(?=.*\d).+$",
      ErrorMessage = "Lösenordet måste innehålla minst en siffra.")]
  public string NewPassword { get; set; } = string.Empty;

  /// <summary>
  /// Bekräftelse av det nya lösenordet.
  /// </summary>
  [Required]
  [Compare(nameof(NewPassword), ErrorMessage = "Lösenorden matchar inte.")]
  public string ConfirmNewPassword { get; set; } = string.Empty;
}
