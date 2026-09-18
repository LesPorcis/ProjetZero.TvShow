using System.Net;

namespace ProjectZero.TvShows.Application.Exceptions;

public abstract class TvShowsCatalogException(HttpStatusCode status, string message)
    : Exception(message)
{
    public HttpStatusCode Status { get; } = status;
}
