using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.Exceptions;

public sealed class TvShowNotFoundException(TvShowId tvShowId)
    : TvShowsApplicationException($"TV show with id {tvShowId.Value} was not found.")
{
    public TvShowId TvShowId { get; } = tvShowId;
}
