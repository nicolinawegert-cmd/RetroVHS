using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Shared.Enums;

namespace RetroVHS.Api.Services.Rentals;

/// <summary>
/// Service som hanterar statusändringar och borttagning av beställningar.
/// </summary>
public class RentalService : IRentalService
{
  private readonly ApplicationDbContext _context;

  public RentalService(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Markerar en beställning som slutförd (Completed).
  /// Användaren kan slutföra sin egen, admin kan slutföra vilken som helst.
  /// </summary>
  public async Task<(bool Success, string Message)> CompleteRentalAsync(int rentalId, int userId, bool isAdmin)
  {
    var rental = await _context.Rentals.FindAsync(rentalId);

    if (rental == null)
      return (false, "Beställningen hittades inte.");

    if (!isAdmin && rental.UserId != userId)
      return (false, "Du har inte behörighet att ändra denna beställning.");

    if (rental.Status == RentalStatus.Completed)
      return (false, "Beställningen är redan slutförd.");

    if (rental.Status == RentalStatus.Cancelled)
      return (false, "En avbruten beställning kan inte slutföras.");

    rental.Status = RentalStatus.Completed;
    await _context.SaveChangesAsync();

    return (true, "Beställningen har markerats som slutförd.");
  }

  /// <summary>
  /// Avbryter en beställning (Cancelled). Endast admin.
  /// Återställer lagersaldo för filmen.
  /// </summary>
  public async Task<(bool Success, string Message)> CancelRentalAsync(int rentalId)
  {
    var rental = await _context.Rentals
        .Include(r => r.Movie)
        .FirstOrDefaultAsync(r => r.Id == rentalId);

    if (rental == null)
      return (false, "Beställningen hittades inte.");

    if (rental.Status == RentalStatus.Cancelled)
      return (false, "Beställningen är redan avbruten.");

    if (rental.Status == RentalStatus.Completed)
      return (false, "En slutförd beställning kan inte avbrytas.");

    rental.Status = RentalStatus.Cancelled;

    // Återställ lagersaldo
    rental.Movie.StockQuantity++;
    if (rental.Movie.AvailabilityStatus == MovieAvailabilityStatus.OutOfStock)
      rental.Movie.AvailabilityStatus = MovieAvailabilityStatus.Available;

    await _context.SaveChangesAsync();

    return (true, "Beställningen har avbrutits och lagersaldot har återställts.");
  }

  /// <summary>
  /// Raderar en beställning.
  /// Användaren kan radera sin egen om den är Cancelled, admin kan radera vilken som helst.
  /// </summary>
  public async Task<(bool Success, string Message)> DeleteRentalAsync(int rentalId, int userId, bool isAdmin)
  {
    var rental = await _context.Rentals.FindAsync(rentalId);

    if (rental == null)
      return (false, "Beställningen hittades inte.");

    if (!isAdmin)
    {
      if (rental.UserId != userId)
        return (false, "Du har inte behörighet att radera denna beställning.");

      if (rental.Status != RentalStatus.Cancelled)
        return (false, "Du kan bara radera avbrutna beställningar.");
    }

    _context.Rentals.Remove(rental);
    await _context.SaveChangesAsync();

    return (true, "Beställningen har raderats.");
  }
}
