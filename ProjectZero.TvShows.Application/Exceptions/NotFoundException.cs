using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.Exceptions;

public sealed class NotFoundException : TvShowsCatalogException
{
    public TvShowId? TvShowId { get; }

    public NotFoundException(TvShowId tvShowId)
        : base($"TV show with id {tvShowId.Value} was not found.")
    {
        TvShowId = tvShowId;
    }

    public NotFoundException(string entityName, IReadOnlyCollection<int> missingIds)
        : base(FormatRelatedEntityMessage(entityName, missingIds)) { }

    private static string FormatRelatedEntityMessage(
        string entityName,
        IReadOnlyCollection<int> missingIds
    )
    {
        var ids = string.Join(", ", missingIds);
        return $"{entityName} with id(s) {ids} was not found.";
    }
}
