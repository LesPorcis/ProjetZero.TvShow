namespace ProjectZero.TvShows.Application.Exceptions;

public sealed class RelatedEntityNotFoundException(string entityName, IReadOnlyCollection<int> missingIds)
    : TvShowsCatalogException(
        $"{entityName} with id(s) {string.Join(", ", missingIds)} was not found.")
{
    public string EntityName { get; } = entityName;

    public IReadOnlyCollection<int> MissingIds { get; } = missingIds;
}
