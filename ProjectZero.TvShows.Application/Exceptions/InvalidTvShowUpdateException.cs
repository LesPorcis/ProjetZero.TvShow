namespace ProjectZero.TvShows.Application.Exceptions;

public sealed class InvalidTvShowUpdateException(string message, Exception? innerException = null)
    : TvShowsCatalogException(message, innerException);
