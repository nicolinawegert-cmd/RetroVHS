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
  /// Tar bort den aktuella användarens recension/kommentar.
  /// </summary>
  Task<bool> RemoveReviewCommentAsync(int userId, int reviewId);

  /// <summary>
  /// Tar bort kommentartexten från en recension men behåller betyget.
  /// Avsett för administrativ moderering.
  /// </summary>
  Task<bool> RemoveReviewCommentAsync(int reviewId);

}
