using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.Auth;

/// <summary>
/// DTO som används när en användare uppdaterar sin egen profilinformation.
/// </summary>
public class UpdateUserProfileDto
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
  /// Valfritt nickname
  /// </summary>
  [StringLength(50)]
  public string? Nickname { get; set; }

  /// <summary>
  /// Användarens e-postadress
  /// </summary>
  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;
}
