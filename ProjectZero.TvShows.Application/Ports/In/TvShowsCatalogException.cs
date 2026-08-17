using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.Ports.In;

public abstract class TvShowsCatalogException(string message)
    : Exception(message);

public sealed class TvShowNotFoundException(TvShowId tvShowId)
    : TvShowsCatalogException($"TV show with id {tvShowId.Value} was not found.")
{
    public TvShowId TvShowId { get; } = tvShowId;
}
