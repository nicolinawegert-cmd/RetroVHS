using Microsoft.AspNetCore.Identity;
using Moq;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Api.Services.Auth;
using RetroVHS.Shared.DTOs.Auth;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace RetroVHS.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IJwtTokenService> _mockJwtTokenSerice;
        private readonly Mock<IConfiguration> _mockConfigartion;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSigninManager;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;
        public AuthServiceTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _mockJwtTokenSerice = new Mock<IJwtTokenService>();
            _mockConfigartion = new Mock<IConfiguration>();
            Mock.Of<IHttpContextAccessor>();
            _mockSigninManager = new Mock<SignInManager<ApplicationUser>>(_mockUserManager.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);


            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new ApplicationDbContext(options);

            _authService = new AuthService(
                _mockUserManager.Object,
                _mockSigninManager.Object, _context,
                _mockJwtTokenSerice.Object,
                _mockConfigartion.Object);
        }
        [Fact]
        public async Task RegisterAsync_EmailAlreadyExists_ReturnsFailed()
        {
            //Arrange
            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync("test@test.com"))
                .ReturnsAsync(new ApplicationUser());
            var request = new RegisterRequestDto { Email = "test@test.com" };

            //Act
            var result = await _authService.RegisterAsync(request);

            //Assert
            Assert.False(result.Succeeded);
        }
        [Fact]
        public async Task RegisterAsync_Success_ReturnsSucceeded()
        {
            //Arrange
            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync("test@test.com"))
                .ReturnsAsync((ApplicationUser?)null);
            _mockUserManager.Setup(userManager => userManager.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockConfigartion.Setup(config => config["Jwt:ExpiresInMinutes"]).Returns("60");
            _mockUserManager.Setup(userManager => userManager.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string>());
            _mockUserManager.Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);
            _mockUserManager.Setup(userManager => userManager.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            //Act
            var request = new RegisterRequestDto { Email = "test@test.com" };
            var result = await _authService.RegisterAsync(request);

            //Assert
            Assert.True(result.Succeeded);
        }
        [Fact]
        public async Task LoginAsync_UserNotFound_ReturnsFailed()
        {
            //Arrange
            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync("test@test.com"))
                   .ReturnsAsync((ApplicationUser?)null);
            //Act
            var request = new LoginRequestDto { Email = "test@test.com" };
            var result = await _authService.LoginAsync(request);

            //Assert
            Assert.False(result.Succeeded);
        }
        [Fact]
        public async Task LoginAsync_UserIsBlocked_ReturnsFailed()
        {
            //Arrange
            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync("test@test.com"))
                   .ReturnsAsync(new ApplicationUser{ IsBlocked = true });
            //Act
            var request = new LoginRequestDto { Email = "test@test.com" };
            var result = await _authService.LoginAsync(request);

            Assert.False(result.Succeeded);
        }
        [Fact]
        public async Task LoginAsync_WrongPassword_ReturnsFailed()
        {
            //Arrange
            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync("test@test.com"))
                   .ReturnsAsync(new ApplicationUser());
            _mockSigninManager.Setup(signinManager => signinManager.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), false))
               .ReturnsAsync(SignInResult.Failed);

            //Act 
            var request = new LoginRequestDto { Email = "test@test.com" };
            var result = await _authService.LoginAsync(request);
            
            //Assert
            Assert.False(result.Succeeded);
        }
        [Fact]
        public async Task LogoutAsync_InvalidToken_ReturnsFailed()
        {
            //Act
            var result = await _authService.LogoutAsync("Invalid-token");

            Assert.False(result.Succeeded);
        }
        [Fact]
        public async Task LogoutAsync_Success_ReturnsSucceeded()
        {
            //Arrange
            _context.RefreshTokens.Add(new RefreshToken { Token = "valid-token" });
            await _context.SaveChangesAsync();

            //Act 
            var result = await _authService.LogoutAsync("valid-token");

            Assert.True(result.Succeeded);
        }
    }
}