using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Api.Services.Reviews;
using RetroVHS.Shared.DTOs.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroVHS.Tests.Services
{
    public class ReviewServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ReviewService _reviewService;

        public ReviewServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ReviewTestDb")
                .Options;
            _context = new ApplicationDbContext(options);
            _reviewService = new ReviewService(_context);
        }

        [Fact]
        public async Task CreateReview_MovieNotFound_ReturnsNull()
        {
            //act
            var result = await _reviewService.CreateReviewAsync(1, new CreateReviewDto { MovieId = 1 });
            //assert
            Assert.Null(result);
        }
        [Fact]
        public async Task RemoveReviewComment_ReviewNotFound_ReturnsFalse()
        {
            //act
            var result = await _reviewService.RemoveReviewCommentAsync(1, 1);
            //assert
            Assert.False(result);
        }
        [Fact]
        public async Task RemoveReviewComment_Success_ReturnsTrue()
        {
            //Arrange
            _context.Reviews.Add(new Review { Id = 1, UserId = 1 });
            await _context.SaveChangesAsync();
            
            //Act
            var result = await _reviewService.RemoveReviewCommentAsync(1, 1);
            
            //Assert
            Assert.True(result);
            
        }


    }   
}
