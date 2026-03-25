using RetroVHS.Shared.DTOs.Admin;
using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Genres;
using RetroVHS.Shared.DTOs.Movies;
using RetroVHS.Shared.DTOs.Persons;
using RetroVHS.Shared.DTOs.ProductionCompanies;
using RetroVHS.Shared.DTOs.Rentals;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Client.Services;

public interface IAdminClient
{
    // Dashboard
    Task<AdminDashboardDto?> GetDashboardStatsAsync();

    // Users
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> BlockUserAsync(int id);
    Task<bool> UnblockUserAsync(int id);
    Task<bool> SetNicknameNullAsync(int id);

    // Reviews
    Task<List<ReviewDto>> GetUserReviewsAsync(int userId);
    Task<bool> RemoveReviewCommentAsync(int reviewId);
    Task<bool> DeleteReviewAsync(int reviewId);

    // Rentals
    Task<List<RentalDto>> GetUserRentalsAsync(int userId);
    Task<bool> CancelRentalAsync(int rentalId);

    // Movies
    Task<List<MovieListDto>> GetAllMoviesAsync();
    Task<MovieDetailsDto?> GetMovieByIdAsync(int id);
    Task<MovieDetailsDto?> CreateMovieAsync(CreateMovieDto dto);
    Task<MovieDetailsDto?> UpdateMovieAsync(int id, UpdateMovieDto dto);
    Task<bool> DeleteMovieAsync(int id);

    // Reference data
    Task<List<GenreDto>> GetGenresAsync();
    Task<List<PersonDto>> GetPersonsAsync(string? search = null);
    Task<List<ProductionCompanyDto>> GetProductionCompaniesAsync();
}
