using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Rentals;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Client.Services;

/// <summary>
/// Klient-sidan för användarprofil — anropar API:t via HTTP.
/// </summary>
public interface IUserClient
{
    Task<UserDto?> GetCurrentUserAsync();
    Task<(UserDto? User, string? Error)> UpdateProfileAsync(UpdateUserProfileDto dto);
    Task<string?> ChangePasswordAsync(ChangePasswordDto dto);
    Task<List<ReviewDto>> GetMyReviewsAsync();
    Task<List<RentalDto>> GetMyOrdersAsync();
    Task<bool> CompleteRentalAsync(int rentalId);
    Task<bool> CancelRentalAsync(int rentalId);
}
