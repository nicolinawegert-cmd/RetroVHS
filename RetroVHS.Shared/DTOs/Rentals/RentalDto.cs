namespace RetroVHS.Shared.DTOs.Rentals;

/// <summary>
/// DTO som representerar en uthyrning.
/// </summary>
public class RentalDto
{
    /// <summary>
    /// Uthyrningens id
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
    /// Pris vid uthyrningstillfället
    /// </summary>
    public decimal PricePaid { get; set; }

    /// <summary>
    /// Starttid för hyran
    /// </summary>
    public DateTime RentedAt { get; set; }

    /// <summary>
    /// När hyran går ut
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Anger om hyran fortfarande är aktiv
    /// </summary>
    public string Status { get; set; } = string.Empty;
}