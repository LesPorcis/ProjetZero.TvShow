namespace ProjectZero.TvShows.Application.Ports.In;

public sealed class CreateTvShowCommand
{
    public required string Name { get; init; }
    public DateOnly? ReleasedAt { get; init; }
    public required int Seasons { get; init; }
    public required int Episodes { get; init; }
}
