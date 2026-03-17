namespace RetroVHS.Shared.DTOs.Rentals;

/// <summary>
/// DTO som returneras efter att en checkout har genomförts.
/// Vi skickar tillbaka om köpet lyckades, ett meddelande till användaren
/// och vilka uthyrningar som skapades.
/// </summary>
public class CheckoutResponseDto
{
    /// <summary>
    /// Anger om checkouten lyckades
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Meddelande som kan visas i frontend
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Lista med skapade uthyrningar
    /// </summary>
    public List<RentalDto> Rentals { get; set; } = new();
}
