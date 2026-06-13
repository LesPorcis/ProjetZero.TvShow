using ProjectZero.Database.Daos;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Mapping;

internal static class WriterMapper
{
    public static Writer ToDomain(this WriterDao dao) => new(dao.Id, dao.FirstName, dao.LastName);

    public static IReadOnlyCollection<Writer> ToDomains(this IEnumerable<WriterDao> daos) =>
        daos.Select(dao => dao.ToDomain()).ToList();
}
