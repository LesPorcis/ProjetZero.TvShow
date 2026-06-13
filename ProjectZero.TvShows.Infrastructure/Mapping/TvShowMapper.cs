using ProjectZero.Database.Daos;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Mapping;

internal static class TvShowMapper
{
    public static TvShow ToDomain(this TvShowDao dao) => new(
        dao.Id,
        dao.Name,
        dao.ReleasedAt,
        dao.Seasons,
        dao.Episodes,
        dao.Directors.ToDomains(),
        dao.Writers.ToDomains(),
        dao.Stars.ToDomains(),
        dao.Genres.ToDomains());

    public static IReadOnlyCollection<TvShow> ToDomains(this IEnumerable<TvShowDao> daos) =>
        daos.Select(dao => dao.ToDomain()).ToList();
}
