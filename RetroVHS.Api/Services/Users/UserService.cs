using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Api.Models;
using Microsoft.AspNetCore.Identity;
using RetroVHS.Shared.DTOs.Reviews;


namespace RetroVHS.Api.Services.Users;

/// <summary>
/// Service som hanterar användarrelaterad affärslogik.
/// </summary>
public class UserService : IUserService
{
  private readonly ApplicationDbContext _context;
  private readonly UserManager<ApplicationUser> _userManager;

  /// <summary>
  /// Skapar en ny instans av servicen och injicerar databaskontexten.
  /// </summary>
  public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
  {
    _context = context;
    _userManager = userManager;
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

  /// <summary>
  /// Uppdaterar profilinformationen för den aktuella användaren.
  /// </summary>
  public async Task<UserDto?> UpdateCurrentUserAsync(int userId, UpdateUserProfileDto dto)
  {
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null)
      return null;

    user.FirstName = dto.FirstName;
    user.LastName = dto.LastName;
    user.Nickname = dto.Nickname;
    user.Email = dto.Email;
    user.UserName = dto.Email;
    user.NormalizedEmail = dto.Email.ToUpper();
    user.NormalizedUserName = dto.Email.ToUpper();
    user.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

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

  /// <summary>
  /// Byter lösenord för den aktuella användaren.
  /// </summary>
  public async Task<(bool Succeeded, List<string> Errors)> ChangePasswordAsync(int userId, ChangePasswordDto dto)
  {
    var user = await _userManager.FindByIdAsync(userId.ToString());

    if (user == null)
    {
      return (false, new List<string> { "Användaren kunde inte hittas." });
    }

    var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

    if (!result.Succeeded)
    {
      return (false, result.Errors.Select(e => e.Description).ToList());
    }

    user.UpdatedAt = DateTime.UtcNow;
    await _userManager.UpdateAsync(user);

    return (true, new List<string>());
  }

  /// <summary>
  /// Hämtar alla recensioner som den aktuella användaren har skrivit.
  /// </summary>
  public async Task<List<ReviewDto>> GetCurrentUserReviewsAsync(int userId)
  {
    return await _context.Reviews
        .Include(r => r.User)
        .Where(r => r.UserId == userId && !r.IsDeleted)
        .OrderByDescending(r => r.CreatedAt)
        .Select(r => new ReviewDto
        {
          Id = r.Id,
          MovieId = r.MovieId,
          UserId = r.UserId,
          UserDisplayName = r.UseNickname && !string.IsNullOrWhiteSpace(r.User.Nickname)
                ? r.User.Nickname!
                : r.User.FullName,
          Conmment = r.Comment ?? string.Empty,
          Rating = r.Rating,
          CreatedAt = r.CreatedAt,
          IsEdited = r.IsEdited
        })
        .ToListAsync();
  }
}
