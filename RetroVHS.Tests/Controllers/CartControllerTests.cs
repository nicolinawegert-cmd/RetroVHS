using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetroVHS.Api.Controllers;
using RetroVHS.Api.Services.Cart;
using RetroVHS.Shared.DTOs.Cart;
using RetroVHS.Shared.DTOs.Rentals;
using Xunit;

namespace RetroVHS.Tests.Controllers
{
    public class CartControllerTests
    {
        private static CartController CreateControllerWithUser(Mock<ICartService> mockService, int userId = 1)
        {
            var controller = new CartController(mockService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "TestAuth"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        [Fact]
        public async Task GetCart_ReturnsOk_WithCartDto()
        {
            // Arrange
            var mockService = new Mock<ICartService>();
            var expected = new CartDto { Id = 1 };
            mockService.Setup(s => s.GetCartAsync(1)).ReturnsAsync(expected);

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.GetCart();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<CartDto>(ok.Value);
            Assert.Equal(expected.Id, returned.Id);
        }

        [Fact]
        public async Task AddToCart_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var mockService = new Mock<ICartService>();
            var dto = new AddToCartDto { MovieId = 5, Quantity = 1 };
            var expected = new CartDto { Id = 1 };
            mockService.Setup(s => s.AddToCartAsync(1, It.IsAny<AddToCartDto>()))
                       .ReturnsAsync(expected);

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.AddToCart(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<CartDto>(ok.Value);
            Assert.Equal(expected.Id, returned.Id);
        }

        [Fact]
        public async Task AddToCart_ReturnsBadRequest_OnInvalidOperationException()
        {
            // Arrange
            var mockService = new Mock<ICartService>();
            mockService.Setup(s => s.AddToCartAsync(1, It.IsAny<AddToCartDto>()))
                       .ThrowsAsync(new InvalidOperationException("Already in cart"));

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.AddToCart(new AddToCartDto { MovieId = 1, Quantity = 1 });

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddToCart_ReturnsNotFound_OnArgumentException()
        {
            // Arrange
            var mockService = new Mock<ICartService>();
            mockService.Setup(s => s.AddToCartAsync(1, It.IsAny<AddToCartDto>()))
                       .ThrowsAsync(new ArgumentException("Movie not found"));

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.AddToCart(new AddToCartDto { MovieId = 999, Quantity = 1 });

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task RemoveFromCart_ReturnsNoContent_WhenRemoved()
        {
            // Arrange
            var mockService = new Mock<ICartService>();
            mockService.Setup(s => s.RemoveFromCartAsync(1, 10)).ReturnsAsync(true);

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.RemoveFromCart(10);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RemoveFromCart_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var mockService = new Mock<ICartService>();
            mockService.Setup(s => s.RemoveFromCartAsync(1, 99)).ReturnsAsync(false);

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.RemoveFromCart(99);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFound.Value);
            Assert.Contains("Raden hittades inte", notFound.Value.ToString());
        }

        [Fact]
        public async Task Checkout_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var mockService = new Mock<ICartService>();
            var dto = new CheckoutDto { PaymentMethod = "card" };
            var response = new CheckoutResponseDto { Success = true };
            mockService.Setup(s => s.CheckoutAsync(1, It.IsAny<CheckoutDto>()))
                       .ReturnsAsync(response);

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.Checkout(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<CheckoutResponseDto>(ok.Value);
            Assert.True(returned.Success);
        }

        [Fact]
        public async Task Checkout_ReturnsBadRequest_WhenFailure()
        {
            // Arrange
            var mockService = new Mock<ICartService>();
            var dto = new CheckoutDto { PaymentMethod = "card" };
            var response = new CheckoutResponseDto { Success = false, Message = "Payment failed" };
            mockService.Setup(s => s.CheckoutAsync(1, It.IsAny<CheckoutDto>()))
                       .ReturnsAsync(response);

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.Checkout(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}