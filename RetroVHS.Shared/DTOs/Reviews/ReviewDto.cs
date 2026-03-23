namespace RetroVHS.Shared.DTOs.Reviews;

/// <summary>
/// DTO som används för att visa en recension i frontend.
/// </summary>
public class ReviewDto
{
    /// <summary>
    /// Recensionens id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Id på filmen recensionen tillhör
    /// </summary>
    public int MovieId { get; set; }

    /// <summary>
    /// Id på användaren som skrivit recensionen
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Användarens namn eller nickname
    /// </summary>
    public string UserDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Själva recensionstexten
    /// </summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Betyg mellan 1 och 5
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Datum då recensionen skapades
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Anger om recensionen har blivit redigerad
    /// </summary>
    public bool IsEdited { get; set; }
}
