using ProjectZero.TvShows.Api.ViewModels;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Api.Mappers;

internal static class TvShowMappings
{
    public static TvShowViewModel ToViewModel(this TvShow series) =>
        new(
            series.Id,
            series.Name,
            series.ReleasedAt,
            series.Seasons,
            series.Episodes);

    public static IReadOnlyCollection<TvShowViewModel> ToViewModels(this IEnumerable<TvShow> series) =>
        series.Select(ToViewModel).ToList();
}
