namespace ProjectZero.TvShows.Application.Exceptions;

/// <summary>
/// Semantic category of a business error, expressed independently of any transport.
/// A driving adapter (HTTP, gRPC, CLI...) is responsible for translating it to its own protocol.
/// </summary>
public enum ErrorKind
{
    NotFound,
    Validation,
    Conflict,
    Unexpected
}
