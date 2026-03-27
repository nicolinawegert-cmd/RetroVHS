using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Api.Services.Wishlists;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroVHS.Tests.Services
{
    public class WishlistServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly WishlistService _wishlistService;

        public WishlistServiceTests()
        {

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: "WishlistTestDb")
                    .Options;
            _context = new ApplicationDbContext(options);
            _wishlistService = new WishlistService(_context);
        }

        [Fact]
        public async Task AddToWishlist_MovieNotFound_ReturnsFalse()
        {
            //Act 
            var result = await _wishlistService.AddToWishlistAsync(1, 2);
            
            //Assert
            Assert.False(result.Success);
        }
        [Fact]
        public async Task AddToWishlist_MovieAlreadyExists_ReturnsFalse()
        {
            //Arrange
            _context.Movies.Add(new Movie {Id = 1});
                await _context.SaveChangesAsync();
            _context.WishlistItems.Add(new WishlistItem { UserId = 1, MovieId = 1});
                await _context.SaveChangesAsync();
            
            //Act
            var result = await _wishlistService.AddToWishlistAsync(1 , 1);

            //Assert
            Assert.False(result.Success);
        }
        [Fact]
        public async Task RemoveFromWishlist_ItemNotFound_ReturnsFalse()
        {
            //Act
            var result = await _wishlistService.RemoveFromWishlistAsync(1, 2);

            //Assert
            Assert.False(result.Success);
        }
    }   
}
