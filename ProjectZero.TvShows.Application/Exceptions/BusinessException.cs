namespace ProjectZero.TvShows.Application.Exceptions;

public abstract class BusinessException(ErrorKind kind, string message)
    : Exception(message)
{
    public ErrorKind Kind { get; } = kind;
}
