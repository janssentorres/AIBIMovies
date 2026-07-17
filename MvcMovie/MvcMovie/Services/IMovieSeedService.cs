namespace MvcMovie.Services;

public interface IMovieSeedService
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}