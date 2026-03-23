using Microsoft.AspNetCore.Mvc;
using Moq;
using RetroVHS.Api.Controllers;
using RetroVHS.Api.Services.Auth;
using RetroVHS.Shared.DTOs.Auth;

namespace RetroVHS.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Login_Success_Returns200Ok()
        {
            // Arrange
            _mockAuthService
                .Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync(new AuthResultDto { Succeeded = true });

            var request = new LoginRequestDto
            {
                Email = "user@retrovhs.com",
                Password = "User123!"
            };
            // Act
            var result = await _controller.Login(request);
            // Assert 
            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task Login_Failure_Returns401Unauthorized()
        {
            // Arrange
            _mockAuthService
                .Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync(new AuthResultDto { Succeeded = false });

            var request = new LoginRequestDto
            {
                Email = "user@retrovhs.com",
                Password = "User123!"
            };
            // Act
            var result = await _controller.Login(request);
            // Assert 
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
        [Fact]
        public async Task Register_Success_Returns200Ok()
        {
            // Arrange
            _mockAuthService
                .Setup(s => s.RegisterAsync(It.IsAny<RegisterRequestDto>()))
                .ReturnsAsync(new AuthResultDto { Succeeded = true });

            var request = new RegisterRequestDto
            {
                Email = "user@retrovhs.com",
                Password = "User123!"
            };
            // Act
            var result = await _controller.Register(request);
            // Assert 
            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task Register_Failure_Returns400BadRequest()
        {
            // Arrange
            _mockAuthService
                .Setup(s => s.RegisterAsync(It.IsAny<RegisterRequestDto>()))
                .ReturnsAsync(new AuthResultDto { Succeeded = false });

            var request = new RegisterRequestDto
            {
                Email = "user@retrovhs.com",
                Password = "User123!"
            };
            // Act
            var result = await _controller.Register(request);
            // Assert 
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
    
}