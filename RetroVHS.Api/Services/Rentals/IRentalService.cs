namespace RetroVHS.Api.Services.Rentals;

/// <summary>
/// Interface för hantering av beställningar (rentals).
/// </summary>
public interface IRentalService
{
  /// <summary>
  /// Markerar en beställning som slutförd (Completed).
  /// Användaren kan slutföra sin egen, admin kan slutföra vilken som helst.
  /// </summary>
  Task<(bool Success, string Message)> CompleteRentalAsync(int rentalId, int userId, bool isAdmin);

  /// <summary>
  /// Avbryter en beställning (Cancelled). Endast admin.
  /// </summary>
  Task<(bool Success, string Message)> CancelRentalAsync(int rentalId);

  /// <summary>
  /// Raderar en beställning.
  /// Användaren kan radera sin egen om den är Cancelled, admin kan radera vilken som helst.
  /// </summary>
  Task<(bool Success, string Message)> DeleteRentalAsync(int rentalId, int userId, bool isAdmin);
}
