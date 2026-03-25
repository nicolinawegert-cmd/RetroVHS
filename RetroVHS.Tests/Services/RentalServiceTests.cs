using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using RetroVHS.Api.Services.Rentals;
using RetroVHS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroVHS.Tests.Services
{
    public class RentalServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly RentalService _rentalService;

        public RentalServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "RentalTestDb")
                .Options;
            _context = new ApplicationDbContext(options);
            _rentalService = new RentalService(_context);
        }
        [Fact]
        public async Task CompleteRental_RentalNotFound_ReturnsFalse()
        {
            //act
            var result = await _rentalService.CompleteRentalAsync(1, 1, false);
            //assert
            Assert.False(result.Success);
        }
       [Fact]
        public async Task CancelRental_RentalNotFound_ReturnsFalse()
        {
            //Act
            var result = await _rentalService.CancelRentalAsync(1);
            Assert.False(result.Success);
        }
        [Fact]
        public async Task DeleteRental_Success_ReturnsTrue()
        {
            //Arrange
            _context.Rentals.Add(new Rental { UserId = 1, Status = RentalStatus.Cancelled });
            await _context.SaveChangesAsync();
            
            //Act
            var result = await _rentalService.DeleteRentalAsync(1,1,false);

            //Assert
            Assert.True(result.Success);
        }

    }
}
