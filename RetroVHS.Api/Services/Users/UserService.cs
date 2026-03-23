using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Shared.DTOs.Auth;

namespace RetroVHS.Api.Services.Users;

/// <summary>
/// Service som hanterar användarrelaterad affärslogik.
/// </summary>
public class UserService : IUserService
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Skapar en ny instans av servicen och injicerar databaskontexten.
  /// </summary>
  public UserService(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Hämtar profilinformationen för den aktuella användaren.
  /// </summary>
  public async Task<UserDto?> GetCurrentUserAsync(int userId)
  {
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null)
      return null;

    return new UserDto
    {
      Id = user.Id,
      FirstName = user.FirstName,
      LastName = user.LastName,
      Nickname = user.Nickname,
      Email = user.Email ?? string.Empty,
      IsBlocked = user.IsBlocked
    };
  }
}
