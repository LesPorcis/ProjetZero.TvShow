namespace ProjectZero.TvShows.Api.ViewModels;

public sealed record TvShowViewModel(
    int Id,
    string Name,
    DateOnly? ReleasedAt,
    int Seasons,
    int Episodes);
