using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Api.Services.Users;
using RetroVHS.Shared.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroVHS.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "UserTestDb")
                .Options;
            _context = new ApplicationDbContext(options);
            _userService = new UserService(_context,_mockUserManager.Object);
        }
        [Fact]
        public async Task GetCurrentUser_UserNotFound_ReturnsNull()
        {
            //Act
            var result = await _userService.GetCurrentUserAsync(999);

            //Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task GetUserById_UserNotFound_ReturnsNull()
        {
            //Act
            var result = await _userService.GetUserByIdAsync(999);
            //Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task ChangePassword_UserNotFound_ReturnsFailed()
        {
            //Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            //Act
            var result = await _userService.ChangePasswordAsync(1, new ChangePasswordDto {CurrentPassword = "OldPassword123!", NewPassword = "NewPassword123!"});

            //Assert
            Assert.False(result.Succeeded);
        }

    }
}
