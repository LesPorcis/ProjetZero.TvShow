namespace ProjectZero.TvShows.Application.Ports.Out;

public sealed class TvShowToCreate
{
    public required string Name { get; init; }
    public DateOnly? ReleasedAt { get; init; }
    public required int Seasons { get; init; }
    public required int Episodes { get; init; }
}
