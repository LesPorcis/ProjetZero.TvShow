namespace ProjectZero.TvShows.Application.Exceptions;

public sealed class InvalidTvShowException(IReadOnlyDictionary<string, string[]> errors)
    : TvShowsApplicationException("The TV show is invalid.")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
