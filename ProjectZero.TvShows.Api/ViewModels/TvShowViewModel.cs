namespace ProjectZero.TvShows.Api.ViewModels;

public sealed class TvShowViewModel
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public DateOnly? ReleasedAt { get; init; }
    public required int Seasons { get; init; }
    public required int Episodes { get; init; }

    public required IReadOnlyCollection<DirectorViewModel> Directors { get; init; }
    public required IReadOnlyCollection<WriterViewModel> Writers { get; init; }
    public required IReadOnlyCollection<StarViewModel> Stars { get; init; }
    public required IReadOnlyCollection<GenreViewModel> Genres { get; init; }
}