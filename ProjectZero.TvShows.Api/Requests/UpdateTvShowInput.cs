namespace ProjectZero.TvShows.Api.Requests;

public sealed class UpdateTvShowInput
{
    public required string Name { get; init; }

    public DateOnly? ReleasedAt { get; init; }

    public required int Seasons { get; init; }

    public required int Episodes { get; init; }

    public required IReadOnlyCollection<int> DirectorIds { get; init; }
    public required IReadOnlyCollection<int> WriterIds { get; init; }
    public required IReadOnlyCollection<int> StarIds { get; init; }
    public required IReadOnlyCollection<int> GenreIds { get; init; }
}
