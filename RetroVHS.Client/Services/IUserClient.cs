using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Rentals;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Client.Services;

/// <summary>
/// Kontrakt för den inloggade användarens profildata och orderhistorik.
/// Alla metoder kräver inloggning (JWT i Authorization-headern).
/// </summary>
public interface IUserClient
{
    /// <summary>GET api/users/me — hämtar profildata för inloggad användare.</summary>
    Task<UserDto?> GetCurrentUserAsync();

    /// <summary>
    /// PUT api/users/me — uppdaterar namn, nickname och e-post.
    /// Returnerar (UserDto, null) vid framgång eller (null, felmeddelande) vid misslyckande.
    /// </summary>
    Task<(UserDto? User, string? Error)> UpdateProfileAsync(UpdateUserProfileDto dto);

    /// <summary>
    /// PUT api/users/me/password — byter lösenord.
    /// Returnerar null vid framgång, annars ett felmeddelande från API:t.
    /// </summary>
    Task<string?> ChangePasswordAsync(ChangePasswordDto dto);

    /// <summary>GET api/users/me/reviews — hämtar alla recensioner skrivna av inloggad användare.</summary>
    Task<List<ReviewDto>> GetMyReviewsAsync();

    /// <summary>GET api/users/me/rentals — hämtar orderhistorik för inloggad användare.</summary>
    Task<List<RentalDto>> GetMyOrdersAsync();

    /// <summary>PUT api/rentals/{rentalId}/complete — markerar en beställning som mottagen.</summary>
    Task<bool> CompleteRentalAsync(int rentalId);

    /// <summary>PUT api/rentals/{rentalId}/cancel — avbryter en aktiv beställning.</summary>
    Task<bool> CancelRentalAsync(int rentalId);
}
