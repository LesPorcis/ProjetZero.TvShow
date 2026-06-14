using ProjectZero.Database.Entities;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Mapping;

internal static class TvShowMapper
{
    public static TvShow ToDomain(this TvShowEntity entity) => new
    (
        entity.Id,
        entity.Name,
        entity.ReleasedAt,
        entity.Seasons,
        entity.Episodes,
        entity.Directors.ToDomains(),
        entity.Writers.ToDomains(),
        entity.Stars.ToDomains(),
        entity.Genres.ToDomains()
    );

    public static IReadOnlyCollection<TvShow> ToDomains(this IEnumerable<TvShowEntity> entities) =>
        entities.Select(entity => entity.ToDomain()).ToList();
}
