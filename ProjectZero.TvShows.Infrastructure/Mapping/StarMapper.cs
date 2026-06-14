using ProjectZero.Database.Daos;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Mapping;

internal static class StarMapper
{
    public static Star ToDomain(this StarDao dao) => new(dao.Person.Id, dao.Person.FirstName, dao.Person.LastName);

    public static IReadOnlyCollection<Star> ToDomains(this IEnumerable<StarDao> daos) =>
        daos.Select(dao => dao.ToDomain()).ToList();
}
