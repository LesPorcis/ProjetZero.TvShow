using ProjectZero.Database.Daos;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Mapping;

internal static class WriterMapper
{
    public static Writer ToDomain(this WriterDao dao) => new(dao.Person.Id, dao.Person.FirstName, dao.Person.LastName);

    public static IReadOnlyCollection<Writer> ToDomains(this IEnumerable<WriterDao> daos) =>
        daos.Select(dao => dao.ToDomain()).ToList();
}
