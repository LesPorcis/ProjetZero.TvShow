using Microsoft.EntityFrameworkCore;
using ProjectZero.Database.Entities;
using ProjectZero.TvShows.Application.Commands;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Mapping;

internal static class TvShowMapper
{
    public static TvShow ToDomain(this TvShowEntity entity) => new
    (
        new TvShowId(entity.Id),
        entity.Name,
        entity.ReleasedAt,
        entity.Seasons,
        entity.Episodes,
        entity.Directors.ToDomains(),
        entity.Writers.ToDomains(),
        entity.Stars.ToDomains(),
        entity.Genres.ToDomains()
    );

    public static IReadOnlyCollection<TvShow> ToDomains(this IReadOnlyCollection<TvShowEntity> entities) =>
        entities.Select(entity => entity.ToDomain()).ToList();

    public static void ApplyUpdate(
        this TvShowEntity entity,
        DbContext dbContext,
        UpdateTvShowCommand command,
        IReadOnlyCollection<GenreEntity> genres)
    {
        dbContext.Entry(entity).Property(tvShow => tvShow.Name).CurrentValue = command.Name;
        dbContext.Entry(entity).Property(tvShow => tvShow.ReleasedAt).CurrentValue = command.ReleasedAt;
        dbContext.Entry(entity).Property(tvShow => tvShow.Seasons).CurrentValue = command.Seasons;
        dbContext.Entry(entity).Property(tvShow => tvShow.Episodes).CurrentValue = command.Episodes;

        ReplacePersonRoles(entity.Directors, command.DirectorIds, personId => new DirectorEntity
        {
            PersonId = personId,
            TvShowId = entity.Id,
        });
        ReplacePersonRoles(entity.Writers, command.WriterIds, personId => new WriterEntity
        {
            PersonId = personId,
            TvShowId = entity.Id,
        });
        ReplacePersonRoles(entity.Stars, command.StarIds, personId => new StarEntity
        {
            PersonId = personId,
            TvShowId = entity.Id,
        });

        entity.Genres.Clear();
        entity.Genres.AddRange(genres);
    }

    private static void ReplacePersonRoles<TEntity>(
        ICollection<TEntity> roles,
        IReadOnlyCollection<int> personIds,
        Func<int, TEntity> createRole)
        where TEntity : class
    {
        roles.Clear();
        foreach (var personId in personIds)
        {
            roles.Add(createRole(personId));
        }
    }
}
