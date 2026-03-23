using RetroVHS.Shared.DTOs.Auth;

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
}
