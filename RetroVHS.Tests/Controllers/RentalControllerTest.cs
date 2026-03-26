using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetroVHS.Api.Controllers;
using RetroVHS.Api.Services.Rentals;
using Xunit;

namespace RetroVHS.Tests.Controllers
{
    public class RentalControllerTest
    {
        private static RentalsController CreateControllerWithUser(Mock<IRentalService> mockService, int userId = 1, bool isAdmin = false)
        {
            var controller = new RentalsController(mockService.Object);

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

        // CompleteRental

        [Fact]
        public async Task CompleteRental_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var mockService = new Mock<IRentalService>();
            mockService.Setup(s => s.CompleteRentalAsync(1, 1, false))
                       .ReturnsAsync((true, "Beställningen har markerats som slutförd."));

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.CompleteRental(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
            Assert.Contains("slutförd", ok.Value.ToString());
        }

        [Fact]
        public async Task CompleteRental_ReturnsBadRequest_WhenFailure()
        {
            // Arrange
            var mockService = new Mock<IRentalService>();
            mockService.Setup(s => s.CompleteRentalAsync(1, 1, false))
                       .ReturnsAsync((false, "Beställningen är redan slutförd."));

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.CompleteRental(1);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(bad.Value);
            Assert.Contains("redan slutförd", bad.Value.ToString());
        }

        // CancelRental

        [Fact]
        public async Task CancelRental_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var mockService = new Mock<IRentalService>();
            mockService.Setup(s => s.CancelRentalAsync(1, 1, false))
                       .ReturnsAsync((true, "Beställningen har avbrutits och lagersaldot har återställts."));

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.CancelRental(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
            Assert.Contains("återställts", ok.Value.ToString());
        }

        [Fact]
        public async Task CancelRental_ReturnsBadRequest_WhenFailure()
        {
            // Arrange
            var mockService = new Mock<IRentalService>();
            mockService.Setup(s => s.CancelRentalAsync(1, 1, false))
                       .ReturnsAsync((false, "Beställningen är redan avbruten."));

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.CancelRental(1);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(bad.Value);
            Assert.Contains("redan avbruten", bad.Value.ToString());
        }

        // DeleteRental

        [Fact]
        public async Task DeleteRental_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var mockService = new Mock<IRentalService>();
            mockService.Setup(s => s.DeleteRentalAsync(1, 1, false))
                       .ReturnsAsync((true, "Beställningen har raderats."));

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.DeleteRental(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
            Assert.Contains("raderats", ok.Value.ToString());
        }

        [Fact]
        public async Task DeleteRental_ReturnsBadRequest_WhenFailure()
        {
            // Arrange
            var mockService = new Mock<IRentalService>();
            mockService.Setup(s => s.DeleteRentalAsync(1, 1, false))
                       .ReturnsAsync((false, "Du har inte behörighet att radera denna beställning."));

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.DeleteRental(1);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(bad.Value);
            Assert.Contains("behörighet", bad.Value.ToString());
        }
    }
}