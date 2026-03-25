using System.Net.Http.Json;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Client.Services;

/// <summary>
/// HTTP-klient för recensionsoperationer.
/// </summary>
public class ReviewClient : IReviewClient
{
    private readonly HttpClient _httpClient;
    public ReviewClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<ReviewDto?> CreateReviewAsync(CreateReviewDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/reviews", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<ReviewDto>()
                : null;
        }
        catch { return null; }
    }

    public async Task<ReviewDto?> UpdateReviewAsync(UpdateReviewDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/reviews/{dto.Id}", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<ReviewDto>()
                : null;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteReviewAsync(int reviewId)
    {
        try { return (await _httpClient.DeleteAsync($"api/admin/reviews/{reviewId}")).IsSuccessStatusCode; }
        catch { return false; }
    }
}
