using Microsoft.AspNetCore.Identity;
using Moq;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Api.Services.Auth;
using RetroVHS.Shared.DTOs.Auth;
using Xunit;


namespace RetroVHS.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IJwtTokenService> _mockJwtTokenSerice;
        private readonly Mock<IConfiguration> _mockConfigartion;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSigninManager;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly ApplicationDbContext _context;
    }
}
