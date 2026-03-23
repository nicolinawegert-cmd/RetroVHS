using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RetroVHS.Api.Controllers;
using RetroVHS.Api.Services.Movies;
using RetroVHS.Shared.DTOs.Movies;

namespace RetroVHS.Tests.Controllers
{
    public class MoviesControllerTests
    {
        //  GET ALL MOVIES
        [Fact]
        public async Task GetMovies_ReturnsOk_WithMovieList()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();

            mockService.Setup(s => s.GetMoviesAsync(It.IsAny<MovieFilterDto>()))
                .ReturnsAsync(new List<MovieListDto>
                {
                    new MovieListDto { Id = 1, Title = "Matrix" }
                });

            var controller = new MoviesController(mockService.Object);

            // Act
            var result = await controller.GetMovies(new MovieFilterDto());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var movies = Assert.IsAssignableFrom<List<MovieListDto>>(okResult.Value);
            Assert.Single(movies);
        }

        //  GET BY ID - FOUND
        [Fact]
        public async Task GetMovieById_ReturnsOk_WhenMovieExists()
        {
            var mockService = new Mock<IMovieService>();

            mockService.Setup(s => s.GetMovieByIdAsync(1))
                .ReturnsAsync(new MovieDetailsDto { Id = 1, Title = "Inception" });

            var controller = new MoviesController(mockService.Object);

            var result = await controller.GetMovieById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var movie = Assert.IsType<MovieDetailsDto>(okResult.Value);
            Assert.Equal("Inception", movie.Title);
        }

        //  GET BY ID - NOT FOUND
        [Fact]
        public async Task GetMovieById_ReturnsNotFound_WhenMovieMissing()
        {
            var mockService = new Mock<IMovieService>();

            mockService.Setup(s => s.GetMovieByIdAsync(1))
                .ReturnsAsync((MovieDetailsDto?)null);

            var controller = new MoviesController(mockService.Object);

            var result = await controller.GetMovieById(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        //  CREATE - SUCCESS
        [Fact]
        public async Task CreateMovie_ReturnsCreatedAtAction_WhenValid()
        {
            var mockService = new Mock<IMovieService>();

            var dto = new CreateMovieDto { Title = "Interstellar" };

            mockService.Setup(s => s.CreateMovieAsync(It.IsAny<CreateMovieDto>()))
                .ReturnsAsync(new MovieDetailsDto { Id = 1, Title = "Interstellar" });

            var controller = new MoviesController(mockService.Object);

            var result = await controller.CreateMovie(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var movie = Assert.IsType<MovieDetailsDto>(created.Value);

            Assert.Equal(1, movie.Id);
        }

        //  CREATE - INVALID MODEL
        [Fact]
        public async Task CreateMovie_ReturnsBadRequest_WhenModelInvalid()
        {
            var mockService = new Mock<IMovieService>();
            var controller = new MoviesController(mockService.Object);

            controller.ModelState.AddModelError("Title", "Required");

            var result = await controller.CreateMovie(new CreateMovieDto());

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        //  UPDATE - ID MISMATCH
        [Fact]
        public async Task UpdateMovie_ReturnsBadRequest_WhenIdMismatch()
        {
            var mockService = new Mock<IMovieService>();
            var controller = new MoviesController(mockService.Object);

            var dto = new UpdateMovieDto { Id = 2 };

            var result = await controller.UpdateMovie(1, dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        //  UPDATE - SUCCESS
        [Fact]
        public async Task UpdateMovie_ReturnsOk_WhenSuccessful()
        {
            var mockService = new Mock<IMovieService>();

            var dto = new UpdateMovieDto { Id = 1, Title = "Updated Movie" };

            mockService.Setup(s => s.UpdateMovieAsync(1, It.IsAny<UpdateMovieDto>()))
                .ReturnsAsync(new MovieDetailsDto { Id = 1, Title = "Updated Movie" });

            var controller = new MoviesController(mockService.Object);

            var result = await controller.UpdateMovie(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var movie = Assert.IsType<MovieDetailsDto>(okResult.Value);

            Assert.Equal("Updated Movie", movie.Title);
        }

        //  UPDATE - NOT FOUND
        [Fact]
        public async Task UpdateMovie_ReturnsNotFound_WhenMovieMissing()
        {
            var mockService = new Mock<IMovieService>();

            mockService.Setup(s => s.UpdateMovieAsync(1, It.IsAny<UpdateMovieDto>()))
                .ReturnsAsync((MovieDetailsDto?)null);

            var controller = new MoviesController(mockService.Object);

            var result = await controller.UpdateMovie(1, new UpdateMovieDto { Id = 1 });

            Assert.IsType<NotFoundResult>(result.Result);
        }

        //  DELETE - SUCCESS
        [Fact]
        public async Task DeleteMovie_ReturnsNoContent_WhenDeleted()
        {
            var mockService = new Mock<IMovieService>();

            mockService.Setup(s => s.DeleteMovieAsync(1))
                .ReturnsAsync(true);

            var controller = new MoviesController(mockService.Object);

            var result = await controller.DeleteMovie(1);

            Assert.IsType<NoContentResult>(result);
        }

        //  DELETE - NOT FOUND
        [Fact]
        public async Task DeleteMovie_ReturnsNotFound_WhenMissing()
        {
            var mockService = new Mock<IMovieService>();

            mockService.Setup(s => s.DeleteMovieAsync(1))
                .ReturnsAsync(false);

            var controller = new MoviesController(mockService.Object);

            var result = await controller.DeleteMovie(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}