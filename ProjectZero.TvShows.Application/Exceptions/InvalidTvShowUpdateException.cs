namespace ProjectZero.TvShows.Application.Exceptions;

public sealed class InvalidTvShowUpdateException : TvShowsCatalogException
{
    private static readonly IReadOnlyDictionary<string, string[]> EmptyErrors =
        new Dictionary<string, string[]>();

    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public InvalidTvShowUpdateException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Errors = EmptyErrors;
    }

    public InvalidTvShowUpdateException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
