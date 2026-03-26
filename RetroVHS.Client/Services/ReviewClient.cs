using System.Net.Http.Json;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Client.Services;

/// <summary>
/// HTTP-klient för recensionsoperationer tillgängliga för inloggade användare.
/// JWT-token skickas automatiskt via Authorization-headern.
/// </summary>
public class ReviewClient : IReviewClient
{
    private readonly HttpClient _httpClient;
    public ReviewClient(HttpClient httpClient) => _httpClient = httpClient;

    // POST api/reviews — skapar recension, returnerar den sparade recensionen med server-genererade fält
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

    // PUT api/reviews/{id} — uppdaterar en befintlig recension, returnerar uppdaterad version
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

}
