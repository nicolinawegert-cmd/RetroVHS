using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Client.Services;

/// <summary>
/// Kontrakt för recensionsoperationer tillgängliga för inloggade användare.
/// Admin-operationer (radera recension) ligger separat i IAdminClient.
/// </summary>
public interface IReviewClient
{
    /// <summary>POST api/reviews — skapar en ny recension. Kräver inloggning.</summary>
    Task<ReviewDto?> CreateReviewAsync(CreateReviewDto dto);

    /// <summary>PUT api/reviews/{id} — uppdaterar betyg och/eller kommentar. Kräver inloggning.</summary>
    Task<ReviewDto?> UpdateReviewAsync(UpdateReviewDto dto);
}
