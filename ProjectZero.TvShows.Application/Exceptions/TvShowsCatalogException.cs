namespace ProjectZero.TvShows.Application.Exceptions;

public abstract class TvShowsCatalogException(ErrorKind kind, string message)
    : Exception(message)
{
    public ErrorKind Kind { get; } = kind;
}
