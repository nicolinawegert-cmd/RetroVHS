using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Client.Services;

/// <summary>
/// Klient för recensionsoperationer — skapa, uppdatera och ta bort.
/// </summary>
public interface IReviewClient
{
    Task<ReviewDto?> CreateReviewAsync(CreateReviewDto dto);
    Task<ReviewDto?> UpdateReviewAsync(UpdateReviewDto dto);
}
