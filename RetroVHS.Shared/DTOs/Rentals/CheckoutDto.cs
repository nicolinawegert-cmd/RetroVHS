using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.Rentals;

/// <summary>
/// DTO som används när användaren genomför en checkout (hyra filmer).
/// Här kan vi senare lägga till betalningsinformation.
/// </summary>
public class CheckoutDto
{
    /// <summary>
    /// Id på varukorgen som ska checkas ut
    /// </summary>
    [Required]
    public int CartId { get; set; }

    /// <summary>
    /// Simulerad betalmetod (kan byggas ut senare)
    /// </summary>
    public string? PaymentMethod { get; set; }
}