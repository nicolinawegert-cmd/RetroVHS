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
    [Required(ErrorMessage = "Förnamn är obligatoriskt.")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Användarens efternamn
    /// </summary>
    [Required(ErrorMessage = "Efternamn är obligatoriskt.")]
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
    [Required(ErrorMessage = "E-post är obligatoriskt.")]
    [EmailAddress(ErrorMessage = "Ogiltig e-postadress.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Lösenord för kontot
    /// </summary>
    [Required(ErrorMessage = "Lösenord är obligatoriskt.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Lösenordet måste vara minst 6 tecken.")]
    [RegularExpression(@"^(?=.*\d).+$",
        ErrorMessage = "Lösenordet måste innehålla minst en siffra.")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Bekräftelse av lösenord för att minska risken för felskrivning
    /// </summary>
    [Required(ErrorMessage = "Bekräfta lösenordet.")]
    [Compare(nameof(Password), ErrorMessage = "Lösenorden matchar inte.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}