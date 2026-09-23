namespace ProjectZero.TvShows.Application.Commands;

public sealed record UpdateTvShowCommand(
    string Name,
    DateOnly? ReleasedAt,
    int Seasons,
    int Episodes,
    IReadOnlyCollection<int> DirectorIds,
    IReadOnlyCollection<int> WriterIds,
    IReadOnlyCollection<int> StarIds,
    IReadOnlyCollection<int> GenreIds);