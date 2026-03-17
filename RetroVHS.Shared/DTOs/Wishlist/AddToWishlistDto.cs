using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.Wishlist;

/// <summary>
/// DTO som används när en användare lägger till en film i sin wishlist.
/// </summary>
public class AddToWishlistDto
{
    /// <summary>
    /// Id på filmen som ska läggas till
    /// </summary>
    [Required]
    public int MovieId { get; set; }
}