using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetroVHS.Api.Controllers;
using RetroVHS.Api.Services.Admin;
using RetroVHS.Api.Services.Reviews;
using RetroVHS.Api.Services.Users;
using RetroVHS.Shared.DTOs.Admin;
using RetroVHS.Shared.DTOs.Auth;
using RetroVHS.Shared.DTOs.Reviews;
using RetroVHS.Shared.DTOs.Rentals;
using System.Security.Claims;
using Xunit;

namespace RetroVHS.Tests.Controllers
{
    public class UserControllerTest
    {
        private static UsersController CreateControllerWithUser(
            Mock<IUserService> userMock,
            Mock<IReviewService> reviewMock,
            Mock<IAdminService> adminMock,
            int userId = 1,
            bool isAdmin = false)
        {
            var controller = new UsersController(userMock.Object, reviewMock.Object, adminMock.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            if (isAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        // GetCurrentUser

        [Fact]
        public async Task GetCurrentUser_ReturnsOk_WhenFound()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            var expected = new UserDto { Id = 1, FirstName = "A", LastName = "B", Email = "a@b" };
            userMock.Setup(s => s.GetCurrentUserAsync(1)).ReturnsAsync(expected);

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1);

            var result = await controller.GetCurrentUser();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<UserDto>(ok.Value);
            Assert.Equal(expected.Id, returned.Id);
        }

        [Fact]
        public async Task GetCurrentUser_ReturnsNotFound_WhenMissing()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            userMock.Setup(s => s.GetCurrentUserAsync(1)).ReturnsAsync((UserDto?)null);

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1);

            var result = await controller.GetCurrentUser();

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // UpdateCurrentUser

        [Fact]
        public async Task UpdateCurrentUser_ReturnsOk_WhenSuccessful()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            var dto = new UpdateUserProfileDto { FirstName = "X", LastName = "Y", Email = "x@y.com" };
            var expected = new UserDto { Id = 1, FirstName = "X", LastName = "Y", Email = "x@y.com" };

            userMock.Setup(s => s.UpdateCurrentUserAsync(1, dto)).ReturnsAsync(expected);

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1);

            var result = await controller.UpdateCurrentUser(dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<UserDto>(ok.Value);
            Assert.Equal(expected.Email, returned.Email);
        }

        [Fact]
        public async Task UpdateCurrentUser_ReturnsNotFound_WhenMissing()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            var dto = new UpdateUserProfileDto { FirstName = "X", LastName = "Y", Email = "x@y.com" };
            userMock.Setup(s => s.UpdateCurrentUserAsync(1, dto)).ReturnsAsync((UserDto?)null);

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1);

            var result = await controller.UpdateCurrentUser(dto);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateCurrentUser_ReturnsBadRequest_OnArgumentException()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            var dto = new UpdateUserProfileDto { FirstName = "X", LastName = "Y", Email = "taken@x.com" };
            userMock.Setup(s => s.UpdateCurrentUserAsync(1, dto))
                    .ThrowsAsync(new System.ArgumentException("E-postadressen används redan av en annan användare."));

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1);

            var result = await controller.UpdateCurrentUser(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // ChangePassword

        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenSucceeded()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            var dto = new ChangePasswordDto { CurrentPassword = "old", NewPassword = "New123", ConfirmNewPassword = "New123" };
            userMock.Setup(s => s.ChangePasswordAsync(1, dto)).ReturnsAsync((true, new List<string>()));

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1);

            var result = await controller.ChangePassword(dto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenFailed()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            var dto = new ChangePasswordDto { CurrentPassword = "old", NewPassword = "short", ConfirmNewPassword = "short" };
            userMock.Setup(s => s.ChangePasswordAsync(1, dto)).ReturnsAsync((false, new List<string> { "Error1" }));

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1);

            var result = await controller.ChangePassword(dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(bad.Value);
        }

        // GetCurrentUserReviews

        [Fact]
        public async Task GetCurrentUserReviews_ReturnsOk()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            var reviews = new List<ReviewDto> { new ReviewDto { Id = 1, Rating = 5 } };
            userMock.Setup(s => s.GetCurrentUserReviewsAsync(1)).ReturnsAsync(reviews);

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1);

            var result = await controller.GetCurrentUserReviews();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<List<ReviewDto>>(ok.Value);
            Assert.Single(returned);
        }

        // GetAllUsers (admin)

        [Fact]
        public async Task GetAllUsers_ReturnsOk_AsAdmin()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            var users = new List<UserDto> { new UserDto { Id = 1, FirstName = "A" } };
            userMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1, isAdmin: true);

            var result = await controller.GetAllUsers();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<List<UserDto>>(ok.Value);
            Assert.Single(returned);
        }

        // GetCurrentUserRentals

        [Fact]
        public async Task GetCurrentUserRentals_ReturnsOk()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            var rentals = new List<RentalDto> { new RentalDto { Id = 1, MovieId = 2 } };
            userMock.Setup(s => s.GetCurrentUserRentalsAsync(1)).ReturnsAsync(rentals);

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1);

            var result = await controller.GetCurrentUserRentals();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<List<RentalDto>>(ok.Value);
            Assert.Single(returned);
        }

        // GetUserById

        [Fact]
        public async Task GetUserById_ReturnsOk_WhenFound()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            userMock.Setup(s => s.GetUserByIdAsync(2)).ReturnsAsync(new UserDto { Id = 2 });

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1, isAdmin: true);

            var result = await controller.GetUserById(2);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<UserDto>(ok.Value);
            Assert.Equal(2, returned.Id);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenMissing()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            userMock.Setup(s => s.GetUserByIdAsync(99)).ReturnsAsync((UserDto?)null);

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1, isAdmin: true);

            var result = await controller.GetUserById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // Admin: RemoveReviewComment

        [Fact]
        public async Task RemoveReviewComment_Admin_ReturnsOk_WhenDeleted()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            reviewMock.Setup(s => s.RemoveReviewCommentAsync(10)).ReturnsAsync(true);

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1, isAdmin: true);

            var result = await controller.RemoveReviewComment(10);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task RemoveReviewComment_Admin_ReturnsNotFound_WhenMissing()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            reviewMock.Setup(s => s.RemoveReviewCommentAsync(99)).ReturnsAsync(false);

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1, isAdmin: true);

            var result = await controller.RemoveReviewComment(99);

            Assert.IsType<NotFoundResult>(result);
        }

        // Admin: GetUserReviewsById / GetUserRentalsById

        [Fact]
        public async Task GetUserReviewsById_ReturnsOk_AsAdmin()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            var reviews = new List<ReviewDto> { new ReviewDto { Id = 5 } };
            userMock.Setup(s => s.GetUserReviewsByIdAsync(2)).ReturnsAsync(reviews);

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1, isAdmin: true);

            var result = await controller.GetUserReviewsById(2);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<List<ReviewDto>>(ok.Value);
            Assert.Single(returned);
        }

        [Fact]
        public async Task GetUserRentalsById_ReturnsOk_AsAdmin()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            var rentals = new List<RentalDto> { new RentalDto { Id = 7 } };
            userMock.Setup(s => s.GetUserRentalsByIdAsync(2)).ReturnsAsync(rentals);

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1, isAdmin: true);

            var result = await controller.GetUserRentalsById(2);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<List<RentalDto>>(ok.Value);
            Assert.Single(returned);
        }

        // Admin: DeleteUser

        [Fact]
        public async Task DeleteUser_Admin_ReturnsOk_WhenSuccessful()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            adminMock.Setup(s => s.DeleteUserAsync(2)).ReturnsAsync((true, "Deleted"));

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1, isAdmin: true);

            var result = await controller.DeleteUser(2);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task DeleteUser_Admin_ReturnsBadRequest_WhenFailure()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            adminMock.Setup(s => s.DeleteUserAsync(2)).ReturnsAsync((false, "Could not delete"));

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1, isAdmin: true);

            var result = await controller.DeleteUser(2);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(bad.Value);
        }

        // Admin: UpdateNickname

        [Fact]
        public async Task UpdateNickname_Admin_ReturnsOk_WhenSuccessful()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            var dto = new AdminSetNicknameDto { Nickname = "newnick" };
            adminMock.Setup(s => s.UpdateNicknameAsync(2, dto)).ReturnsAsync((true, "OK"));

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1, isAdmin: true);

            var result = await controller.UpdateNickname(2, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task UpdateNickname_Admin_ReturnsBadRequest_WhenFailure()
        {
            var userMock = new Mock<IUserService>();
            var reviewMock = new Mock<IReviewService>();
            var adminMock = new Mock<IAdminService>();

            var dto = new AdminSetNicknameDto { Nickname = "newnick" };
            adminMock.Setup(s => s.UpdateNicknameAsync(2, dto)).ReturnsAsync((false, "Bad"));

            var controller = CreateControllerWithUser(userMock, reviewMock, adminMock, 1, isAdmin: true);

            var result = await controller.UpdateNickname(2, dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(bad.Value);
        }
    }
}
