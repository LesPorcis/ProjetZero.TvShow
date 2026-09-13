namespace ProjectZero.TvShows.Application.Exceptions;

public abstract class TvShowsCatalogException(string message, Exception? innerException = null)
    : Exception(message, innerException);
