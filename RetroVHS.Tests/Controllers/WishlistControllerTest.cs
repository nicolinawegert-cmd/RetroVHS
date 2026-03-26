using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetroVHS.Api.Controllers;
using RetroVHS.Api.Services.Wishlists;
using RetroVHS.Shared.DTOs.Wishlist;
using Xunit;

namespace RetroVHS.Tests.Controllers
{
    public class WishlistControllerTest
    {
        private static WishlistController CreateControllerWithUser(Mock<IWishlistService> mockService, int userId = 1)
        {
            var controller = new WishlistController(mockService.Object);

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

        // GetWishlist

        [Fact]
        public async Task GetWishlist_ReturnsOk_WithItems()
        {
            // Arrange
            var mockService = new Mock<IWishlistService>();
            var items = new List<WishlistItemDto>
            {
                new WishlistItemDto { MovieId = 1, Title = "M1" }
            };

            mockService.Setup(s => s.GetWishlistAsync(1)).ReturnsAsync(items);

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.GetWishlist();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<List<WishlistItemDto>>(ok.Value);
            Assert.Single(returned);
            Assert.Equal(1, returned[0].MovieId);
        }

        // AddToWishlist

        [Fact]
        public async Task AddToWishlist_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var mockService = new Mock<IWishlistService>();
            mockService.Setup(s => s.AddToWishlistAsync(1, 5))
                       .ReturnsAsync((true, "Filmen har lagts till i din önskelista."));

            var controller = CreateControllerWithUser(mockService, 1);
            var dto = new AddToWishlistDto { MovieId = 5 };

            // Act
            var result = await controller.AddToWishlist(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task AddToWishlist_ReturnsBadRequest_WhenFailure()
        {
            // Arrange
            var mockService = new Mock<IWishlistService>();
            mockService.Setup(s => s.AddToWishlistAsync(1, 99))
                       .ReturnsAsync((false, "Filmen hittades inte."));

            var controller = CreateControllerWithUser(mockService, 1);
            var dto = new AddToWishlistDto { MovieId = 99 };

            // Act
            var result = await controller.AddToWishlist(dto);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(bad.Value);
        }

        // RemoveFromWishlist

        [Fact]
        public async Task RemoveFromWishlist_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var mockService = new Mock<IWishlistService>();
            mockService.Setup(s => s.RemoveFromWishlistAsync(1, 5))
                       .ReturnsAsync((true, "Filmen har tagits bort från din önskelista."));

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.RemoveFromWishlist(5);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task RemoveFromWishlist_ReturnsBadRequest_WhenFailure()
        {
            // Arrange
            var mockService = new Mock<IWishlistService>();
            mockService.Setup(s => s.RemoveFromWishlistAsync(1, 99))
                       .ReturnsAsync((false, "Filmen finns inte i din önskelista."));

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.RemoveFromWishlist(99);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(bad.Value);
        }
    }
}