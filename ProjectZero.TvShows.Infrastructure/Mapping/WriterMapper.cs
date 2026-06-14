using ProjectZero.Database.Entities;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Mapping;

internal static class WriterMapper
{
    public static Writer ToDomain(this WriterEntity entity) => 
        new(entity.Person.Id, entity.Person.FirstName, entity.Person.LastName);

    public static IReadOnlyCollection<Writer> ToDomains(this IReadOnlyCollection<WriterEntity> entities) =>
        entities.Select(entity => entity.ToDomain()).ToList();
}
