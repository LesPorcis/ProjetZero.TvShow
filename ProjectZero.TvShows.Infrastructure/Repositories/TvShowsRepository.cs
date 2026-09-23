using Microsoft.EntityFrameworkCore;
using ProjectZero.Database;
using ProjectZero.Database.Entities;
using ProjectZero.TvShows.Application.Commands;
using ProjectZero.TvShows.Application.Exceptions;
using ProjectZero.TvShows.Application.Ports.Out;
using ProjectZero.TvShows.Domain;
using ProjectZero.TvShows.Infrastructure.Mapping;

namespace ProjectZero.TvShows.Infrastructure.Repositories;

internal sealed class TvShowsRepository(TvShowDbContext dbContext) : ITvShowsRepository
{
    private IQueryable<TvShowEntity> ReadOnlySet() =>
        dbContext
            .Set<TvShowEntity>()
            .AsNoTracking()
            .Include(tvShow => tvShow.Directors)
                .ThenInclude(director => director.Person)
            .Include(tvShow => tvShow.Writers)
                .ThenInclude(writer => writer.Person)
            .Include(tvShow => tvShow.Stars)
                .ThenInclude(star => star.Person)
            .Include(tvShow => tvShow.Genres);

    private IQueryable<TvShowEntity> TrackedSet() =>
        dbContext
            .Set<TvShowEntity>()
            .Include(tvShow => tvShow.Directors)
            .Include(tvShow => tvShow.Writers)
            .Include(tvShow => tvShow.Stars)
            .Include(tvShow => tvShow.Genres);

    public async Task<IReadOnlyCollection<TvShow>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        var tvShowEntities = await ReadOnlySet().ToListAsync(cancellationToken);

        return tvShowEntities.ToDomains();
    }

    public async Task<TvShow?> FindByIdAsync(
        TvShowId id,
        CancellationToken cancellationToken = default
    )
    {
        var tvShowEntity = await ReadOnlySet()
            .SingleOrDefaultAsync(tvShow => tvShow.Id == id.Value, cancellationToken);

        return tvShowEntity?.ToDomain();
    }

    public async Task<TvShow?> UpdateAsync(
        TvShowId id,
        UpdateTvShowCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var tvShowToUpdate = await TrackedSet()
            .SingleOrDefaultAsync(tvShow => tvShow.Id == id.Value, cancellationToken);

        if (tvShowToUpdate is null)
        {
            return null;
        }

        var genres = await dbContext
            .Set<GenreEntity>()
            .Where(genre => command.GenreIds.Contains(genre.Id))
            .ToListAsync(cancellationToken);

        tvShowToUpdate.ApplyUpdate(dbContext, command, genres);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidTvShowUpdateException(
                "The TV show could not be updated because one or more related entities are invalid.",
                exception
            );
        }

        return await FindByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<int>> FindExistingPersonIdsAsync(
        IReadOnlyCollection<int> personIds,
        CancellationToken cancellationToken = default
    )
    {
        if (personIds.Count == 0)
        {
            return [];
        }

        return await dbContext
            .Set<PersonEntity>()
            .AsNoTracking()
            .Where(person => personIds.Contains(person.Id))
            .Select(person => person.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<int>> FindExistingGenreIdsAsync(
        IReadOnlyCollection<int> genreIds,
        CancellationToken cancellationToken = default
    )
    {
        if (genreIds.Count == 0)
        {
            return [];
        }

        return await dbContext
            .Set<GenreEntity>()
            .AsNoTracking()
            .Where(genre => genreIds.Contains(genre.Id))
            .Select(genre => genre.Id)
            .ToListAsync(cancellationToken);
    }
}
