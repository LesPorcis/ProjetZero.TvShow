using Microsoft.EntityFrameworkCore;
using ProjectZero.Database;
using ProjectZero.Database.Entities;
using ProjectZero.TvShows.Application.Ports.Out;
using ProjectZero.TvShows.Domain;
using ProjectZero.TvShows.Infrastructure.Mapping;

namespace ProjectZero.TvShows.Infrastructure.Repositories;

internal sealed class TvShowsRepository(TvShowDbContext dbContext) : ITvShowsRepository
{
    private IQueryable<TvShowEntity> ReadOnlySet() => dbContext.Set<TvShowEntity>()
        .AsNoTracking()
        .Include(tvShow => tvShow.Directors).ThenInclude(director => director.Person)
        .Include(tvShow => tvShow.Writers).ThenInclude(writer => writer.Person)
        .Include(tvShow => tvShow.Stars).ThenInclude(star => star.Person)
        .Include(tvShow => tvShow.Genres);

    public async Task<IReadOnlyCollection<TvShow>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tvShowEntities = await ReadOnlySet()
            .ToListAsync(cancellationToken);

        return tvShowEntities.ToDomains();
    }

    public async Task<TvShow?> FindByIdAsync(TvShowId id, CancellationToken cancellationToken = default)
    {
        var tvShowEntity = await ReadOnlySet()
            .SingleOrDefaultAsync(tvShow => tvShow.Id == id.Value, cancellationToken);

        return tvShowEntity?.ToDomain();
    }

    public async Task<TvShow> CreateAsync(
        string name,
        DateOnly? releasedAt,
        int seasons,
        int episodes,
        CancellationToken cancellationToken = default)
    {
        var tvShowEntity = new TvShowEntity
        {
            Name = name,
            ReleasedAt = releasedAt,
            Seasons = seasons,
            Episodes = episodes
        };

        dbContext.Set<TvShowEntity>().Add(tvShowEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return tvShowEntity.ToDomain();
    }
}
