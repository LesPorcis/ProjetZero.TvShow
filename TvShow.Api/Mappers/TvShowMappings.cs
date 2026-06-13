using TvShow.Api.ViewModels;
using TvShow.Application.Models;

namespace TvShow.Api.Mappers;

/// <summary>
/// Mapping simple et explicite DTO applicatif → ViewModel d'API.
/// </summary>
internal static class TvShowMappings
{
    public static TvShowViewModel ToViewModel(this TvShowResponse response) =>
        new(
            response.Id,
            response.Name,
            response.ReleasedAt,
            response.Seasons,
            response.Episodes);

    public static IReadOnlyList<TvShowViewModel> ToViewModels(this IEnumerable<TvShowResponse> responses) =>
        responses.Select(ToViewModel).ToList();
}
