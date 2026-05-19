using TvShow.Application.Ports;
using TvShowEntity = TvShow.Domain.TvShow;

namespace TvShow.Infrastructure.Repositories;

public sealed class InMemoryTvShowRepository : ITvShowRepository
{
    private static readonly IReadOnlyCollection<TvShowEntity> TvShows =
    [
        new()
        {
            Id = 1,
            Name = "Breaking Bad",
            ReleasedAt = new DateOnly(2008, 1, 20),
            Seasons = 5,
            Episodes = 62
        },
        new()
        {
            Id = 2,
            Name = "The Last of Us",
            ReleasedAt = new DateOnly(2023, 1, 15),
            Seasons = 2,
            Episodes = 16
        }
    ];

    public Task<IReadOnlyCollection<TvShowEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(TvShows);
    }
}
