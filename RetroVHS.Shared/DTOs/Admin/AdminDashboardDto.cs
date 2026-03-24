namespace RetroVHS.Shared.DTOs.Admin;

/// <summary>
/// DTO som innehåller övergripande statistik för admin-dashboarden.
/// </summary>
public class AdminDashboardDto
{
    /// <summary>
    /// Totalt antal registrerade användare
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Antal blockerade användare
    /// </summary>
    public int BlockedUsers { get; set; }

    /// <summary>
    /// Totalt antal filmer i katalogen
    /// </summary>
    public int TotalMovies { get; set; }

    /// <summary>
    /// Antal aktiva uthyrningar
    /// </summary>
    public int ActiveRentals { get; set; }

    /// <summary>
    /// Totalt antal uthyrningar
    /// </summary>
    public int TotalRentals { get; set; }

    /// <summary>
    /// Totalt antal recensioner
    /// </summary>
    public int TotalReviews { get; set; }
}
