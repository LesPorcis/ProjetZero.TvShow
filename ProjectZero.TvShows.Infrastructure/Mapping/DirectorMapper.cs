using ProjectZero.Database.Daos;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Mapping;

internal static class DirectorMapper
{
    public static Director ToDomain(this DirectorDao dao) => new(dao.Id, dao.FirstName, dao.LastName);
}
