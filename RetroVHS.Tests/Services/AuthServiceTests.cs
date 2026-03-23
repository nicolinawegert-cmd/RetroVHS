using Microsoft.AspNetCore.Identity;
using Moq;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Api.Services.Auth;
using RetroVHS.Shared.DTOs.Auth;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

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
            _mockSigninManager = new Mock<SignInManager<ApplicationUser>>(
            _mockUserManager.Object, null, null, null, null, null, null);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new ApplicationDbContext(options);

            _authService = new AuthService(
                _mockUserManager.Object,
                _mockSigninManager.Object,
                _context,
                _mockJwtTokenSerice.Object,
                _mockConfigartion.Object);


        }
    }
}