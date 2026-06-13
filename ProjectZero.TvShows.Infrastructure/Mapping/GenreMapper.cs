using ProjectZero.Database.Daos;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Mapping;

internal static class GenreMapper
{
    public static Genre ToDomain(this GenreDao dao) => new(dao.Id, dao.Name, dao.Description);

    public static IReadOnlyCollection<Genre> ToDomains(this IEnumerable<GenreDao> daos) =>
        daos.Select(dao => dao.ToDomain()).ToList();
}
