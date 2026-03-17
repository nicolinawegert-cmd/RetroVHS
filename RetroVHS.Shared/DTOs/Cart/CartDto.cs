namespace RetroVHS.Shared.DTOs.Cart;

/// <summary>
/// DTO som representerar hela varukorgen för en användare.
/// </summary>
public class CartDto
{
    /// <summary>
    /// Varukorgens id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Lista med alla filmer i varukorgen
    /// </summary>
    public List<CartItemDto> Items { get; set; } = new();

    /// <summary>
    /// Totalt antal filmer i varukorgen
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Totalt pris för alla filmer i varukorgen
    /// </summary>
    public decimal TotalPrice { get; set; }
}