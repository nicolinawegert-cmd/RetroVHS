namespace RetroVHS.Shared.Enums;

/// <summary>
/// Describes the status of a rental.
/// This helps us track if a rental is still active,
/// has expired, or was cancelled.
/// </summary>
public enum RentalStatus
{
    Active = 1,
    Expired = 2,
    Cancelled = 3,
    Completed = 4
}