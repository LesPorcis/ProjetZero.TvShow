using Microsoft.EntityFrameworkCore;
using ProjectZero.Database;
using ProjectZero.Database.Daos;
using ProjectZero.TvShows.Application.Ports.Out;
using ProjectZero.TvShows.Domain;
using ProjectZero.TvShows.Infrastructure.Mapping;

namespace ProjectZero.TvShows.Infrastructure.Repositories;

internal sealed class TvShowsRepository(TvShowDbContext dbContext) : ITvShowsRepository
{
    private IQueryable<TvShowDao> ReadOnlySet() => dbContext.Set<TvShowDao>()
        .AsNoTracking()
        .Include(tvShow => tvShow.Directors).ThenInclude(director => director.Person)
        .Include(tvShow => tvShow.Writers).ThenInclude(writer => writer.Person)
        .Include(tvShow => tvShow.Stars).ThenInclude(star => star.Person)
        .Include(tvShow => tvShow.Genres);

    public async Task<IReadOnlyCollection<TvShow>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tvShowDaos = await ReadOnlySet()
            .ToListAsync(cancellationToken);

        return tvShowDaos.ToDomains();
    }
}
