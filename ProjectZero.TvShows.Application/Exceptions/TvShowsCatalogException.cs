namespace ProjectZero.TvShows.Application.Exceptions;

public abstract class TvShowsCatalogException : Exception
{
    protected TvShowsCatalogException(string message)
        : base(message)
    {
    }

    protected TvShowsCatalogException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
