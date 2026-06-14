using ProjectZero.Database.Entities;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Mapping;

internal static class StarMapper
{
    public static Star ToDomain(this StarEntity entity) => 
        new(entity.Person.Id, entity.Person.FirstName, entity.Person.LastName);

    public static IReadOnlyCollection<Star> ToDomains(this IEnumerable<StarEntity> entities) =>
        entities.Select(entity => entity.ToDomain()).ToList();
}
