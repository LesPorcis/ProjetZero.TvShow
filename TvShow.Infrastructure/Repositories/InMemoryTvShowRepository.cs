using TvShow.Application.Ports.Out;
using TvShow.Domain;

namespace TvShow.Infrastructure.Repositories;

public sealed class InMemoryTvShowRepository : ITvShowRepository
{
    private static readonly IReadOnlyCollection<Series> Shows =
    [
        new Series(1, "Breaking Bad", new DateOnly(2008, 1, 20), 5, 62),
        new Series(2, "The Last of Us", new DateOnly(2023, 1, 15), 2, 16)
    ];

    public Task<IReadOnlyCollection<Series>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Shows);
    }
}
