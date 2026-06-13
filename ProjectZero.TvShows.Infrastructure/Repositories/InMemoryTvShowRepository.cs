using ProjectZero.TvShows.Application.Ports.Out;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Repositories;

public sealed class InMemoryTvShowRepository : ITvShowRepository
{
    private static readonly IReadOnlyCollection<TvShow> Shows =
    [
        new TvShow(1, "Breaking Bad", new DateOnly(2008, 1, 20), 5, 62),
        new TvShow(2, "The Last of Us", new DateOnly(2023, 1, 15), 2, 16)
    ];

    public Task<IReadOnlyCollection<TvShow>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Shows);
    }
}
