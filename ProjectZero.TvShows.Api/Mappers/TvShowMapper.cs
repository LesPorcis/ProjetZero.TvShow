using ProjectZero.TvShows.Api.ViewModels;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Api.Mappers;

internal static class TvShowMapper
{
    public static TvShowViewModel ToViewModel(this TvShow series) => new()
    {
        Id = series.Id,
        Name = series.Name,
        ReleasedAt = series.ReleasedAt,
        Seasons = series.Seasons,
        Episodes = series.Episodes
    };

    public static IReadOnlyCollection<TvShowViewModel> ToViewModels(this IReadOnlyCollection<TvShow> series) =>
        series.Select(ToViewModel).ToList();
}
