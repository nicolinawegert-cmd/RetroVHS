using Microsoft.AspNetCore.Mvc;
using Moq;
using RetroVHS.Api.Controllers;
using RetroVHS.Api.Services.Admin;
using RetroVHS.Api.Services.Movies;
using RetroVHS.Shared.DTOs.Admin;
using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Rentals;
using RetroVHS.Shared.DTOs.Reviews;

namespace RetroVHS.Tests.Controllers;

public class AdminControllerTests
{
  private static AdminController CreateController(
      Mock<IAdminService> adminService,
      Mock<IMovieService>? movieService = null)
  {
    movieService ??= new Mock<IMovieService>();
    return new AdminController(adminService.Object, movieService.Object);
  }

  [Fact]
  public async Task GetAllUsers_ReturnsOk_WithUsers()
  {
    var adminService = new Mock<IAdminService>();
    adminService.Setup(s => s.GetAllUsersAsync())
        .ReturnsAsync(new List<UserDto> { new() { Id = 1, FirstName = "A" } });

    var controller = CreateController(adminService);
    var result = await controller.GetAllUsers();

    var ok = Assert.IsType<OkObjectResult>(result.Result);
    var users = Assert.IsType<List<UserDto>>(ok.Value);
    Assert.Single(users);
  }

  [Fact]
  public async Task GetUserById_ReturnsOk_WhenFound()
  {
    var adminService = new Mock<IAdminService>();
    adminService.Setup(s => s.GetUserByIdAsync(2))
        .ReturnsAsync(new UserDto { Id = 2, FirstName = "User" });

    var controller = CreateController(adminService);
    var result = await controller.GetUserById(2);

    var ok = Assert.IsType<OkObjectResult>(result.Result);
    var user = Assert.IsType<UserDto>(ok.Value);
    Assert.Equal(2, user.Id);
  }

  [Fact]
  public async Task GetUserById_ReturnsNotFound_WhenMissing()
  {
    var adminService = new Mock<IAdminService>();
    adminService.Setup(s => s.GetUserByIdAsync(99)).ReturnsAsync((UserDto?)null);

    var controller = CreateController(adminService);
    var result = await controller.GetUserById(99);

    Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public async Task GetUserReviews_ReturnsOk_WithReviews()
  {
    var adminService = new Mock<IAdminService>();
    adminService.Setup(s => s.GetUserReviewsAsync(2))
        .ReturnsAsync(new List<ReviewDto> { new() { Id = 5, Rating = 4 } });

    var controller = CreateController(adminService);
    var result = await controller.GetUserReviews(2);

    var ok = Assert.IsType<OkObjectResult>(result.Result);
    var reviews = Assert.IsType<List<ReviewDto>>(ok.Value);
    Assert.Single(reviews);
  }

  [Fact]
  public async Task GetUserRentals_ReturnsOk_WithRentals()
  {
    var adminService = new Mock<IAdminService>();
    adminService.Setup(s => s.GetUserRentalsAsync(2))
        .ReturnsAsync(new List<RentalDto> { new() { Id = 7, MovieId = 4 } });

    var controller = CreateController(adminService);
    var result = await controller.GetUserRentals(2);

    var ok = Assert.IsType<OkObjectResult>(result.Result);
    var rentals = Assert.IsType<List<RentalDto>>(ok.Value);
    Assert.Single(rentals);
  }

  [Fact]
  public async Task DeleteUser_ReturnsOk_WhenSuccessful()
  {
    var adminService = new Mock<IAdminService>();
    adminService.Setup(s => s.DeleteUserAsync(2)).ReturnsAsync((true, "Deleted"));

    var controller = CreateController(adminService);
    var result = await controller.DeleteUser(2);

    Assert.IsType<OkObjectResult>(result);
  }

  [Fact]
  public async Task DeleteUser_ReturnsBadRequest_WhenFailed()
  {
    var adminService = new Mock<IAdminService>();
    adminService.Setup(s => s.DeleteUserAsync(2)).ReturnsAsync((false, "Could not delete"));

    var controller = CreateController(adminService);
    var result = await controller.DeleteUser(2);

    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task UpdateNickname_ReturnsOk_WhenSuccessful()
  {
    var adminService = new Mock<IAdminService>();
    var dto = new AdminSetNicknameDto { Nickname = "newnick" };

    adminService.Setup(s => s.UpdateNicknameAsync(2, dto)).ReturnsAsync((true, "OK"));

    var controller = CreateController(adminService);
    var result = await controller.UpdateNickname(2, dto);

    Assert.IsType<OkObjectResult>(result);
  }

  [Fact]
  public async Task UpdateNickname_ReturnsBadRequest_WhenFailed()
  {
    var adminService = new Mock<IAdminService>();
    var dto = new AdminSetNicknameDto { Nickname = "newnick" };

    adminService.Setup(s => s.UpdateNicknameAsync(2, dto)).ReturnsAsync((false, "Bad"));

    var controller = CreateController(adminService);
    var result = await controller.UpdateNickname(2, dto);

    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task RemoveReviewComment_ReturnsOk_WhenSuccessful()
  {
    var adminService = new Mock<IAdminService>();
    adminService.Setup(s => s.RemoveReviewCommentAsync(10)).ReturnsAsync((true, "Done"));

    var controller = CreateController(adminService);
    var result = await controller.RemoveReviewComment(10);

    Assert.IsType<OkObjectResult>(result);
  }

  [Fact]
  public async Task RemoveReviewComment_ReturnsBadRequest_WhenFailed()
  {
    var adminService = new Mock<IAdminService>();
    adminService.Setup(s => s.RemoveReviewCommentAsync(10)).ReturnsAsync((false, "Missing"));

    var controller = CreateController(adminService);
    var result = await controller.RemoveReviewComment(10);

    Assert.IsType<BadRequestObjectResult>(result);
  }
}
