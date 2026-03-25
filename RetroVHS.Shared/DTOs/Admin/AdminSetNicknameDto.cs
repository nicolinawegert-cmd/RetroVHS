using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.Admin;

/// <summary>
/// DTO som används när admin sätter ett nytt nickname på en användare.
/// </summary>
public class AdminSetNicknameDto
{
    /// <summary>
    /// Nytt nickname för användaren. Kan vara null för att ta bort nicknmanet.
    /// </summary>
    [StringLength(50)]
    public string? Nickname { get; set; }
}
