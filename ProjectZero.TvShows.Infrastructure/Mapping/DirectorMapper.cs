using ProjectZero.Database.Daos;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Mapping;

internal static class DirectorMapper
{
    public static Director ToDomain(this DirectorDao dao) => new(dao.Person.Id, dao.Person.FirstName, dao.Person.LastName);

    public static IReadOnlyCollection<Director> ToDomains(this IEnumerable<DirectorDao> daos) =>
        daos.Select(dao => dao.ToDomain()).ToList();
}
