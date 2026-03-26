using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.Cart;

/// <summary>
/// DTO som används när en användare lägger till en film i varukorgen.
/// </summary>
public class AddToCartDto
{
    /// <summary>
    /// Id på filmen som ska läggas till
    /// </summary>
    [Required]
    public int MovieId { get; set; }

    /// <summary>
    /// Antal (normalt 1, max 10)
    /// </summary>
    [Range(1, 10)]
    public int Quantity { get; set; } = 1;
}