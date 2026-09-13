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
    private IQueryable<TvShowEntity> ReadOnlySet() => dbContext.Set<TvShowEntity>()
        .AsNoTracking()
        .Include(tvShow => tvShow.Directors).ThenInclude(director => director.Person)
        .Include(tvShow => tvShow.Writers).ThenInclude(writer => writer.Person)
        .Include(tvShow => tvShow.Stars).ThenInclude(star => star.Person)
        .Include(tvShow => tvShow.Genres);

    private IQueryable<TvShowEntity> WritableSet() => dbContext.Set<TvShowEntity>()
        .Include(tvShow => tvShow.Directors)
        .Include(tvShow => tvShow.Writers)
        .Include(tvShow => tvShow.Stars)
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

    public async Task<TvShow?> UpdateAsync(
        TvShowId id,
        UpdateTvShowCommand command,
        CancellationToken cancellationToken = default)
    {
        var entity = await WritableSet()
            .SingleOrDefaultAsync(tvShow => tvShow.Id == id.Value, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        await EnsureAllPersonsExistAsync(command.DirectorIds, cancellationToken);
        await EnsureAllPersonsExistAsync(command.WriterIds, cancellationToken);
        await EnsureAllPersonsExistAsync(command.StarIds, cancellationToken);
        await EnsureAllGenresExistAsync(command.GenreIds, cancellationToken);

        dbContext.Entry(entity).Property(tvShow => tvShow.Name).CurrentValue = command.Name;
        dbContext.Entry(entity).Property(tvShow => tvShow.ReleasedAt).CurrentValue = command.ReleasedAt;
        dbContext.Entry(entity).Property(tvShow => tvShow.Seasons).CurrentValue = command.Seasons;
        dbContext.Entry(entity).Property(tvShow => tvShow.Episodes).CurrentValue = command.Episodes;

        ReplacePersonRoles(entity.Directors, command.DirectorIds, personId => new DirectorEntity
        {
            PersonId = personId,
            TvShowId = entity.Id
        });
        ReplacePersonRoles(entity.Writers, command.WriterIds, personId => new WriterEntity
        {
            PersonId = personId,
            TvShowId = entity.Id
        });
        ReplacePersonRoles(entity.Stars, command.StarIds, personId => new StarEntity
        {
            PersonId = personId,
            TvShowId = entity.Id
        });

        var genres = await dbContext.Set<GenreEntity>()
            .Where(genre => command.GenreIds.Contains(genre.Id))
            .ToListAsync(cancellationToken);

        entity.Genres.Clear();
        entity.Genres.AddRange(genres);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidTvShowUpdateException(
                "The TV show could not be updated because one or more related entities are invalid.",
                exception);
        }

        return await FindByIdAsync(id, cancellationToken);
    }

    private async Task EnsureAllGenresExistAsync(
        IReadOnlyCollection<int> genreIds,
        CancellationToken cancellationToken)
    {
        var distinctIds = genreIds.Distinct().ToList();
        if (distinctIds.Count == 0)
        {
            return;
        }

        var foundIds = await dbContext.Set<GenreEntity>()
            .Where(genre => distinctIds.Contains(genre.Id))
            .Select(genre => genre.Id)
            .ToListAsync(cancellationToken);

        ThrowIfAnyMissing("Genre", distinctIds, foundIds);
    }

    private async Task EnsureAllPersonsExistAsync(
        IReadOnlyCollection<int> personIds,
        CancellationToken cancellationToken)
    {
        var distinctIds = personIds.Distinct().ToList();
        if (distinctIds.Count == 0)
        {
            return;
        }

        var foundIds = await dbContext.Set<PersonEntity>()
            .Where(person => distinctIds.Contains(person.Id))
            .Select(person => person.Id)
            .ToListAsync(cancellationToken);

        ThrowIfAnyMissing("Person", distinctIds, foundIds);
    }

    private static void ThrowIfAnyMissing(string entityName, IReadOnlyCollection<int> requestedIds, IReadOnlyCollection<int> foundIds)
    {
        var missingIds = requestedIds.Except(foundIds).ToList();
        if (missingIds.Count > 0)
        {
            throw new RelatedEntityNotFoundException(entityName, missingIds);
        }
    }

    private static void ReplacePersonRoles<TEntity>(
        ICollection<TEntity> roles,
        IReadOnlyCollection<int> personIds,
        Func<int, TEntity> createRole)
        where TEntity : class
    {
        roles.Clear();
        foreach (var personId in personIds.Distinct())
        {
            roles.Add(createRole(personId));
        }
    }
}
