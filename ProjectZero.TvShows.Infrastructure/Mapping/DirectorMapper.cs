using ProjectZero.Database.Entities;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Mapping;

internal static class DirectorMapper
{
    public static Director ToDomain(this DirectorEntity entity) => 
        new(entity.Person.Id, entity.Person.FirstName, entity.Person.LastName);

    public static IReadOnlyCollection<Director> ToDomains(this IReadOnlyCollection<DirectorEntity> entities) =>
        entities.Select(entity => entity.ToDomain()).ToList();
}
