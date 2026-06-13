using TvShow.Api.ViewModels;

namespace TvShow.Api.Mappers;

/// <summary>
/// Mapping simple et explicite entité de domaine → ViewModel d'API.
/// </summary>
internal static class TvShowMappings
{
    public static TvShowViewModel ToViewModel(this TvShow.Domain.TvShow tvShow) =>
        new(
            tvShow.Id,
            tvShow.Name,
            tvShow.ReleasedAt,
            tvShow.Seasons,
            tvShow.Episodes);

    public static IReadOnlyCollection<TvShowViewModel> ToViewModels(this IEnumerable<TvShow.Domain.TvShow> tvShows) =>
        tvShows.Select(ToViewModel).ToList();
}
