
namespace RetroVHS.Shared.Enums;

/// <summary>
/// Describes whether a movie can currently be rented or viewed in the catalog.
/// We use this instead of plain text strings to reduce typo risks
/// and make filtering/sorting easier in both backend and frontend.
/// </summary>
public enum MovieAvailabilityStatus
{
    Available = 1,
    OutOfStock = 2,
    ComingSoon = 3,
    NotAvailable = 4
}