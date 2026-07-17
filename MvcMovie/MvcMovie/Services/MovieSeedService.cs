using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Services;

public sealed class MovieSeedService(MvcMovieContext context, ILogger<MovieSeedService> logger) : IMovieSeedService
{
    private const int RequiredMovieCount = 1000;

    private static readonly string[] Genres =
    [
        "Action",
        "Comedy",
        "Drama",
        "Horror"
    ];

    private static readonly string[] Ratings =
    [
        "G",
        "PG",
        "R"
    ];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await context.Database.MigrateAsync(cancellationToken); //This fix the Updating of Database incase of new changes.

        var existingMovieCount = await context.Movie.CountAsync(cancellationToken);

        if (existingMovieCount >= RequiredMovieCount)
        {
            logger.LogInformation("Movie database already contains {MovieCount} records.", existingMovieCount);
            return;
        }

        var moviesToCreate = RequiredMovieCount - existingMovieCount;

        var movies = GenerateMovies(moviesToCreate, existingMovieCount);

        await context.Movie.AddRangeAsync(movies, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {MovieCount} movies. Database now contains {TotalCount} movies.", moviesToCreate, RequiredMovieCount);
    }

    private static IEnumerable<Movie> GenerateMovies(int count, int existingMovieCount)
    {
        for (var index = 1; index <= count; index++)
        {
            var movieNumber = existingMovieCount + index;

            yield return new Movie
            {
                Title = $"AIBIMovie {movieNumber:0000}",
                Genre = Genres[(movieNumber - 1) % Genres.Length],
                ReleaseDate = CreateReleaseDate(movieNumber),
                Price = CreatePrice(movieNumber),
                Rating = Ratings[(movieNumber - 1) % Ratings.Length]
            };
        }
    }

    private static DateTime CreateReleaseDate(int movieNumber)
    {
        var year = 1991 + movieNumber % 76;
        var month = movieNumber % 12 + 1;
        var day = movieNumber % 27 + 1;

        return new DateTime(year, month, day);
    }

    private static decimal CreatePrice(int movieNumber)
    {
        return 4.99m + movieNumber % 25;
    }
}