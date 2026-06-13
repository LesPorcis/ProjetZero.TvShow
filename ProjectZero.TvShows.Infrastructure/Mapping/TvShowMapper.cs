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
        dao.Directors.Select(d => d.ToDomain()).ToList(),
        dao.Writers.Select(w => w.ToDomain()).ToList(),
        dao.Stars.Select(s => s.ToDomain()).ToList(),
        dao.Genres.Select(g => g.ToDomain()).ToList());

    private static Writer ToDomain(this WriterDao dao) => new(dao.Id, dao.FirstName, dao.LastName);

    private static Star ToDomain(this StarDao dao) => new(dao.Id, dao.FirstName, dao.LastName);

    public static Genre ToDomain(this GenreDao dao) => new(dao.Id, dao.Name, dao.Description);
}
