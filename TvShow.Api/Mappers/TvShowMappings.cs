using TvShow.Api.ViewModels;
using TvShow.Domain;

namespace TvShow.Api.Mappers;

/// <summary>
/// Mapping simple et explicite entité de domaine → ViewModel d'API.
/// </summary>
internal static class TvShowMappings
{
    public static TvShowViewModel ToViewModel(this Series series) =>
        new(
            series.Id,
            series.Name,
            series.ReleasedAt,
            series.Seasons,
            series.Episodes);

    public static IReadOnlyCollection<TvShowViewModel> ToViewModels(this IEnumerable<Series> series) =>
        series.Select(ToViewModel).ToList();
}
