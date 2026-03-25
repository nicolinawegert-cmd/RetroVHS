using Microsoft.Extensions.Configuration;
using Moq;
using RetroVHS.Api.Models;
using RetroVHS.Api.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroVHS.Tests.Services
{
    public class JwtTokenServiceTests
    {
        private readonly JwtTokenService _jwtTokenService;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public JwtTokenServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _jwtTokenService = new JwtTokenService(_mockConfiguration.Object);
        }
        [Fact]
        public async Task GenerateAccessToken_ValidUser_ReturnsToken()
        {
            //Arrange
            _mockConfiguration.Setup(config => config["Jwt:Key"]).Returns("test-secret-key-1234567890123456");
            _mockConfiguration.Setup(config => config["Jwt:ExpiresInMinutes"]).Returns("60");
            _mockConfiguration.Setup(config => config["Jwt:Issuer"]).Returns("test-issuer");
            _mockConfiguration.Setup(config => config["Jwt:Audience"]).Returns("test-audience");

            //Act
            var user = new ApplicationUser();
            var list = new List<string>();
            var result = _jwtTokenService.GenerateAccessToken(user, list);
            
            //Assert
            Assert.NotEmpty(result);
        }
    }
    
}
