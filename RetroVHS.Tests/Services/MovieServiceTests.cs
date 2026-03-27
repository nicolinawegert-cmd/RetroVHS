using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Services.Movies;
using RetroVHS.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroVHS.Tests.Services
{
    public class MovieServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly MovieService _movieService;

        public MovieServiceTests()
        {
            
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: "MovieTestDb")
                    .Options;
            _context = new ApplicationDbContext(options);
            _movieService = new MovieService(_context);
        }
        [Fact]
        public async Task GetMovieById_MovieNotFound_ReturnsNull()
        {
            //Act
            var result = await _movieService.GetMovieByIdAsync(1);

            //Assert
            Assert.Null(result);   
        }
        [Fact]
        public async Task DeleteMovie_MovieNotFound_ReturnsFalse()
        {
            //act 
            var result = await _movieService.DeleteMovieAsync(1);

            //Assert
            Assert.False(result);
        }
        [Fact]
        public async Task DeleteMovie_Success_ReturnsTrue()
        {
            //Arrange
            _context.Movies.Add(new Movie { Id = 1 });
            await _context.SaveChangesAsync();

            //Act
            var result = await _movieService.DeleteMovieAsync(1);

            //Assert
            Assert.True(result);
        }
    }
}

