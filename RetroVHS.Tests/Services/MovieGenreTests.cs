using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
using Xunit;

namespace RetroVHS.Tests
{
    public class MovieGenreTests
    {
        private ApplicationDbContext GetDbContext()
        {
            // Skapar en ny InMemory DB för varje test
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Can_Create_MovieGenre_Object()
        {
            // Arrange
            var movie = new Movie { Id = 1, Title = "Matrix" };
            var genre = new Genre { Id = 1, Name = "Sci-Fi" };

            // Act
            var movieGenre = new MovieGenre
            {
                MovieId = movie.Id,
                GenreId = genre.Id,
                Movie = movie,
                Genre = genre
            };

            // Assert
            Assert.Equal(1, movieGenre.MovieId);
            Assert.Equal(1, movieGenre.GenreId);
            Assert.Equal("Matrix", movieGenre.Movie.Title);
            Assert.Equal("Sci-Fi", movieGenre.Genre.Name);
        }

        [Fact]
        public void Can_Add_MovieGenre_To_Database()
        {
            // Arrange
            using var context = GetDbContext();

            var movie = new Movie { Id = 1, Title = "Matrix" };
            var genre = new Genre { Id = 1, Name = "Sci-Fi" };

            context.Movies.Add(movie);
            context.Genres.Add(genre);
            context.SaveChanges();

            var movieGenre = new MovieGenre
            {
                MovieId = movie.Id,
                GenreId = genre.Id
            };

            // Act
            context.MovieGenres.Add(movieGenre);
            context.SaveChanges();

            // Assert
            var savedRelation = context.MovieGenres.FirstOrDefault();
            Assert.NotNull(savedRelation);
            Assert.Equal(1, savedRelation.MovieId);
            Assert.Equal(1, savedRelation.GenreId);
        }

        [Fact]
        public void Can_Relate_Movie_With_Multiple_Genres()
        {
            // Arrange
            using var context = GetDbContext();

            var movie = new Movie { Id = 1, Title = "Matrix" };
            var genre1 = new Genre { Id = 1, Name = "Sci-Fi" };
            var genre2 = new Genre { Id = 2, Name = "Action" };

            context.Movies.Add(movie);
            context.Genres.AddRange(genre1, genre2);
            context.SaveChanges();

            context.MovieGenres.AddRange(
                new MovieGenre { MovieId = movie.Id, GenreId = genre1.Id },
                new MovieGenre { MovieId = movie.Id, GenreId = genre2.Id }
            );
            context.SaveChanges();

            // Act
            var genresOfMovie = context.MovieGenres
                .Where(mg => mg.MovieId == movie.Id)
                .Select(mg => mg.GenreId)
                .ToList();

            // Assert
            Assert.Contains(1, genresOfMovie);
            Assert.Contains(2, genresOfMovie);
            Assert.Equal(2, genresOfMovie.Count);
        }
    }
}
