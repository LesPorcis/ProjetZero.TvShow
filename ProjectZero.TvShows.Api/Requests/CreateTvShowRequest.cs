namespace ProjectZero.TvShows.Api.Requests;

public sealed class CreateTvShowRequest
{
    public required string Name { get; init; }
    public DateOnly? ReleasedAt { get; init; }
    public required int Seasons { get; init; }
    public required int Episodes { get; init; }
}
