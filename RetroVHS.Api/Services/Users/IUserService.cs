using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Api.Services.Users;

/// <summary>
/// Interface för användarservice som hanterar profilrelaterad logik.
/// </summary>
public interface IUserService
{
  /// <summary>
  /// Hämtar profilinformationen för den aktuella användaren.
  /// </summary>
  Task<UserDto?> GetCurrentUserAsync(int userId);

  /// <summary>
  /// Uppdaterar profilinformationen för den aktuella användaren.
  /// </summary>
  Task<UserDto?> UpdateCurrentUserAsync(int userId, UpdateUserProfileDto dto);

  /// <summary>
  /// Byter lösenord för den aktuella användaren.
  /// </summary>
  Task<(bool Succeeded, List<string> Errors)> ChangePasswordAsync(int userId, ChangePasswordDto dto);

  /// <summary>
  /// Hämtar alla recensioner som den aktuella användaren har skrivit.
  /// </summary>
  Task<List<ReviewDto>> GetCurrentUserReviewsAsync(int userId);

  /// <summary>
  /// Hämtar profilinformationen för en specifik användare.
  /// </summary>
  Task<UserDto?> GetUserByIdAsync(int userId);

  /// <summary>
  /// Tar bort kommentartexten från en recension men behåller betyget.
  /// Endast avsett för administrativ moderering.
  /// </summary>
  Task<bool> RemoveReviewCommentAsync(int reviewId);

}
