using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Services.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroVHS.Tests.Services
{
    public class AdminServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly AdminService _adminService;

        public AdminServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AdminTestDb")
                .Options;
            _context = new ApplicationDbContext(options);
            _adminService = new AdminService(_context);
        }
        [Fact]
        public async Task GetUserById_UserNotFound_ReturnsNull()
        {
            //Act
            var result = await _adminService.GetUserByIdAsync(999);

            //Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task BlockUser_UserNotFound_ReturnsFalse()
        {
           //Act
            var result = await _adminService.BlockUserAsync(999);
            
            //Assert
            Assert.False(result.Success);
        }
         [Fact]
         public async Task DeleteUser_UserNotFound_ReturnsFalse()
        {
            //Act
            var result = await _adminService.DeleteUserAsync(999);

            //Assert
            Assert.False(result.Success);
        }
    }
}