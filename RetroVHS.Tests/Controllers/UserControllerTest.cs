using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetroVHS.Api.Controllers;
using RetroVHS.Api.Services.Users;
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
            int userId = 1,
            bool isAdmin = false)
        {
            var controller = new UsersController(userMock.Object);

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

            var expected = new UserDto { Id = 1, FirstName = "A", LastName = "B", Email = "a@b" };
            userMock.Setup(s => s.GetCurrentUserAsync(1)).ReturnsAsync(expected);

            var controller = CreateControllerWithUser(userMock, 1);

            var result = await controller.GetCurrentUser();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<UserDto>(ok.Value);
            Assert.Equal(expected.Id, returned.Id);
        }

        [Fact]
        public async Task GetCurrentUser_ReturnsNotFound_WhenMissing()
        {
            var userMock = new Mock<IUserService>();

            userMock.Setup(s => s.GetCurrentUserAsync(1)).ReturnsAsync((UserDto?)null);

            var controller = CreateControllerWithUser(userMock, 1);

            var result = await controller.GetCurrentUser();

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // UpdateCurrentUser

        [Fact]
        public async Task UpdateCurrentUser_ReturnsOk_WhenSuccessful()
        {
            var userMock = new Mock<IUserService>();

            var dto = new UpdateUserProfileDto { FirstName = "X", LastName = "Y", Email = "x@y.com" };
            var expected = new UserDto { Id = 1, FirstName = "X", LastName = "Y", Email = "x@y.com" };

            userMock.Setup(s => s.UpdateCurrentUserAsync(1, dto)).ReturnsAsync(expected);

            var controller = CreateControllerWithUser(userMock, 1);

            var result = await controller.UpdateCurrentUser(dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<UserDto>(ok.Value);
            Assert.Equal(expected.Email, returned.Email);
        }

        [Fact]
        public async Task UpdateCurrentUser_ReturnsNotFound_WhenMissing()
        {
            var userMock = new Mock<IUserService>();

            var dto = new UpdateUserProfileDto { FirstName = "X", LastName = "Y", Email = "x@y.com" };
            userMock.Setup(s => s.UpdateCurrentUserAsync(1, dto)).ReturnsAsync((UserDto?)null);

            var controller = CreateControllerWithUser(userMock, 1);

            var result = await controller.UpdateCurrentUser(dto);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateCurrentUser_ReturnsBadRequest_OnArgumentException()
        {
            var userMock = new Mock<IUserService>();

            var dto = new UpdateUserProfileDto { FirstName = "X", LastName = "Y", Email = "taken@x.com" };
            userMock.Setup(s => s.UpdateCurrentUserAsync(1, dto))
                    .ThrowsAsync(new System.ArgumentException("E-postadressen används redan av en annan användare."));

            var controller = CreateControllerWithUser(userMock, 1);

            var result = await controller.UpdateCurrentUser(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // ChangePassword

        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenSucceeded()
        {
            var userMock = new Mock<IUserService>();

            var dto = new ChangePasswordDto { CurrentPassword = "old", NewPassword = "New123", ConfirmNewPassword = "New123" };
            userMock.Setup(s => s.ChangePasswordAsync(1, dto)).ReturnsAsync((true, new List<string>()));

            var controller = CreateControllerWithUser(userMock, 1);

            var result = await controller.ChangePassword(dto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenFailed()
        {
            var userMock = new Mock<IUserService>();

            var dto = new ChangePasswordDto { CurrentPassword = "old", NewPassword = "short", ConfirmNewPassword = "short" };
            userMock.Setup(s => s.ChangePasswordAsync(1, dto)).ReturnsAsync((false, new List<string> { "Error1" }));

            var controller = CreateControllerWithUser(userMock, 1);

            var result = await controller.ChangePassword(dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(bad.Value);
        }

        // GetCurrentUserReviews

        [Fact]
        public async Task GetCurrentUserReviews_ReturnsOk()
        {
            var userMock = new Mock<IUserService>();

            var reviews = new List<ReviewDto> { new ReviewDto { Id = 1, Rating = 5 } };
            userMock.Setup(s => s.GetCurrentUserReviewsAsync(1)).ReturnsAsync(reviews);

            var controller = CreateControllerWithUser(userMock, 1);

            var result = await controller.GetCurrentUserReviews();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<List<ReviewDto>>(ok.Value);
            Assert.Single(returned);
        }

        // GetCurrentUserRentals

        [Fact]
        public async Task GetCurrentUserRentals_ReturnsOk()
        {
            var userMock = new Mock<IUserService>();

            var rentals = new List<RentalDto> { new RentalDto { Id = 1, MovieId = 2 } };
            userMock.Setup(s => s.GetCurrentUserRentalsAsync(1)).ReturnsAsync(rentals);

            var controller = CreateControllerWithUser(userMock, 1);

            var result = await controller.GetCurrentUserRentals();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<List<RentalDto>>(ok.Value);
            Assert.Single(returned);
        }

    }
}
