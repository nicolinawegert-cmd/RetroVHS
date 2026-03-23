using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Api.Services.Reviews;

/// <summary>
/// Interface för review-service som hanterar recensioner och betyg.
/// </summary>
public interface IReviewService
{
  /// <summary>
  /// Skapar en ny recension för en film från den aktuella användaren.
  /// </summary>
  Task<ReviewDto?> CreateReviewAsync(int userId, CreateReviewDto dto);

  /// <summary>
  /// Uppdaterar en befintlig recension för den aktuella användaren.
  /// </summary>
  Task<ReviewDto?> UpdateReviewAsync(int userId, int reviewId, UpdateReviewDto dto);

  /// <summary>
  /// Tar bort den aktuella användarens recension genom mjuk borttagning.
  /// </summary>
  Task<bool> DeleteReviewAsync(int userId, int reviewId);

}
