using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.Exceptions;

public sealed class TvShowNotFoundException(TvShowId tvShowId)
    : BusinessException(ErrorKind.NotFound, $"TV show with id {tvShowId.Value} was not found.")
{
    public TvShowId TvShowId { get; } = tvShowId;
}
