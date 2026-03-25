using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Api.Services.Cart;
using RetroVHS.Shared.DTOs.Rentals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroVHS.Tests.Services
{
    public class CartServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CartTestDb")
                .Options;
            _context = new ApplicationDbContext(options);
            _cartService = new CartService(_context);
        }
        [Fact]
        public async Task Checkout_EmptyCart_ReturnsFalse()
        {
            //Act
            var result = await _cartService.CheckoutAsync(1, new CheckoutDto { CartId = 1 });
            
            //Assert
            Assert.False(result.Success);
        }
        [Fact]
        public async Task RemoveFromCart_ItemNotFound_ReturnsFalse()
        {
           //Act
            var result = await _cartService.RemoveFromCartAsync(1, 2);
            
            //Assert
            Assert.False(result);
        }
         [Fact]
         public async Task GetCart_NewUser_ReturnsEmptyCart()
        {
            //Act
            var result = await _cartService.GetCartAsync(1);
            
            //Assert
            Assert.NotNull(result.Items);
        }
    }
}
