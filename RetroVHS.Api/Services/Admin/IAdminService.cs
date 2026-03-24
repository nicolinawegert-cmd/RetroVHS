using RetroVHS.Shared.DTOs.Admin;
using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Rentals;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Api.Services.Admin;

/// <summary>
/// Interface för admin-service som hanterar all administrativ affärslogik.
/// </summary>
public interface IAdminService
{
    // ========== Dashboard ==========

    /// <summary>
    /// Hämtar övergripande statistik för admin-dashboarden.
    /// </summary>
    Task<AdminDashboardDto> GetDashboardStatsAsync();

    // ========== Användare ==========

    /// <summary>
    /// Hämtar alla användare i systemet.
    /// </summary>
    Task<List<UserDto>> GetAllUsersAsync();

    /// <summary>
    /// Hämtar en specifik användare.
    /// </summary>
    Task<UserDto?> GetUserByIdAsync(int userId);

    /// <summary>
    /// Uppdaterar en användares namn och e-post.
    /// </summary>
    Task<UserDto?> UpdateUserAsync(int userId, UpdateUserProfileDto dto);

    /// <summary>
    /// Raderar en användare och all relaterad data.
    /// </summary>
    Task<(bool Success, string Message)> DeleteUserAsync(int userId);

    /// <summary>
    /// Blockerar en användare.
    /// </summary>
    Task<(bool Success, string Message)> BlockUserAsync(int userId);

    /// <summary>
    /// Avblockerar en användare.
    /// </summary>
    Task<(bool Success, string Message)> UnblockUserAsync(int userId);

    // ========== Recensioner ==========

    /// <summary>
    /// Hämtar alla recensioner för en specifik användare.
    /// </summary>
    Task<List<ReviewDto>> GetUserReviewsAsync(int userId);

    /// <summary>
    /// Redigerar en recensionskommentar och/eller betyg.
    /// </summary>
    Task<(bool Success, string Message)> UpdateReviewAsync(int reviewId, AdminUpdateReviewDto dto);

    /// <summary>
    /// Tar bort kommentartexten men behåller betyget.
    /// </summary>
    Task<(bool Success, string Message)> RemoveReviewCommentAsync(int reviewId);

    /// <summary>
    /// Raderar en recension helt (soft-delete).
    /// </summary>
    Task<(bool Success, string Message)> DeleteReviewAsync(int reviewId);

    // ========== Beställningar ==========

    /// <summary>
    /// Hämtar alla beställningar för en specifik användare.
    /// </summary>
    Task<List<RentalDto>> GetUserRentalsAsync(int userId);

    /// <summary>
    /// Avbryter en aktiv beställning och återställer lagersaldo.
    /// </summary>
    Task<(bool Success, string Message)> CancelRentalAsync(int rentalId);
}
