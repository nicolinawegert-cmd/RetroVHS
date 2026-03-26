using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetroVHS.Api.Controllers;
using RetroVHS.Api.Services.Reviews;
using RetroVHS.Shared.DTOs.Reviews;
using Xunit;

namespace RetroVHS.Tests.Controllers
{
    public class ReviewsControllerTest
    {
        private static ReviewsController CreateControllerWithUser(Mock<IReviewService> mockService, int userId = 1)
        {
            var controller = new ReviewsController(mockService.Object);

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

        // ========== CreateReview ==========

        [Fact]
        public async Task CreateReview_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var dto = new CreateReviewDto { MovieId = 1, Rating = 5, Comment = "Bra film!" };
            var expected = new ReviewDto { Id = 1, MovieId = 1, Rating = 5, Comment = "Bra film!" };

            mockService.Setup(s => s.CreateReviewAsync(1, It.IsAny<CreateReviewDto>()))
                       .ReturnsAsync(expected);

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.CreateReview(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<ReviewDto>(ok.Value);
            Assert.Equal(expected.Id, returned.Id);
            Assert.Equal(expected.Rating, returned.Rating);
        }

        [Fact]
        public async Task CreateReview_ReturnsNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            mockService.Setup(s => s.CreateReviewAsync(1, It.IsAny<CreateReviewDto>()))
                       .ReturnsAsync((ReviewDto?)null);

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.CreateReview(new CreateReviewDto { MovieId = 999, Rating = 3 });

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateReview_ReturnsBadRequest_OnArgumentException()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            mockService.Setup(s => s.CreateReviewAsync(1, It.IsAny<CreateReviewDto>()))
                       .ThrowsAsync(new ArgumentException("Du har redan recenserat den här filmen."));

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.CreateReview(new CreateReviewDto { MovieId = 1, Rating = 4 });

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // ========== UpdateReview ==========

        [Fact]
        public async Task UpdateReview_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var dto = new UpdateReviewDto { Id = 1, Rating = 4, Comment = "Uppdaterad!" };
            var expected = new ReviewDto { Id = 1, Rating = 4, Comment = "Uppdaterad!", IsEdited = true };

            mockService.Setup(s => s.UpdateReviewAsync(1, 1, It.IsAny<UpdateReviewDto>()))
                       .ReturnsAsync(expected);

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.UpdateReview(1, dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<ReviewDto>(ok.Value);
            Assert.Equal(expected.Rating, returned.Rating);
            Assert.True(returned.IsEdited);
        }

        [Fact]
        public async Task UpdateReview_ReturnsNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var dto = new UpdateReviewDto { Id = 1, Rating = 3, Comment = "Test" };

            mockService.Setup(s => s.UpdateReviewAsync(1, 1, It.IsAny<UpdateReviewDto>()))
                       .ReturnsAsync((ReviewDto?)null);

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.UpdateReview(1, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateReview_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var dto = new UpdateReviewDto { Id = 5, Rating = 3, Comment = "Test" };

            var controller = CreateControllerWithUser(mockService, 1);

            // Act — route id = 1, dto.Id = 5 → mismatch
            var result = await controller.UpdateReview(1, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // ========== RemoveReviewComment ==========

        [Fact]
        public async Task RemoveReviewComment_ReturnsNoContent_WhenDeleted()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            mockService.Setup(s => s.RemoveReviewCommentAsync(1, 10))
                       .ReturnsAsync(true);

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.RemoveReviewComment(10);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RemoveReviewComment_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            mockService.Setup(s => s.RemoveReviewCommentAsync(1, 99))
                       .ReturnsAsync(false);

            var controller = CreateControllerWithUser(mockService, 1);

            // Act
            var result = await controller.RemoveReviewComment(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}