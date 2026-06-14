using ProjectZero.Database.Entities;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Mapping;

internal static class GenreMapper
{
    public static Genre ToDomain(this GenreEntity entity) => 
        new(entity.Id, entity.Name, entity.Description);

    public static IReadOnlyCollection<Genre> ToDomains(this IReadOnlyCollection<GenreEntity> entities) =>
        entities.Select(entity => entity.ToDomain()).ToList();
}
