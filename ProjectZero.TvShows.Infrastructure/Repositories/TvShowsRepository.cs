using Microsoft.EntityFrameworkCore;
using ProjectZero.Database;
using ProjectZero.Database.Daos;
using ProjectZero.TvShows.Application.Ports.Out;
using ProjectZero.TvShows.Domain;
using ProjectZero.TvShows.Infrastructure.Mapping;

namespace ProjectZero.TvShows.Infrastructure.Repositories;

internal sealed class TvShowsRepository(TvShowDbContext dbContext) : ITvShowsRepository
{
    private IQueryable<TvShowDao> ReadOnlySet() => dbContext.TvShows
        .AsNoTracking()
        .Include(tvShow => tvShow.Directors)
        .Include(tvShow => tvShow.Writers)
        .Include(tvShow => tvShow.Stars)
        .Include(tvShow => tvShow.Genres);

    public async Task<IReadOnlyCollection<TvShow>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var daos = await ReadOnlySet()
            .ToListAsync(cancellationToken);

        return daos.ToDomains();
    }
}
