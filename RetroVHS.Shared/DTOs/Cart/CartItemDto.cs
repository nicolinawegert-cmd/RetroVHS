namespace RetroVHS.Shared.DTOs.Cart;

/// <summary>
/// DTO som representerar en film i varukorgen.
/// </summary>
public class CartItemDto
{
    /// <summary>
    /// Id för raden i varukorgen
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Filmens id
    /// </summary>
    public int MovieId { get; set; }

    /// <summary>
    /// Filmens titel
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Posterbild
    /// </summary>
    public string? PosterUrl { get; set; }

    /// <summary>
    /// Pris per film
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Antal (oftast 1)
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// När filmen lades till i varukorgen
    /// </summary>
    public DateTime AddedAt { get; set; }
}