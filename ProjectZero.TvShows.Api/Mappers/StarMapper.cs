using ProjectZero.TvShows.Api.ViewModels;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Api.Mappers;

internal static class StarMapper
{
    public static StarViewModel ToViewModel(this Star star) => new()
    {
        Id = star.Id,
        FirstName = star.FirstName,
        LastName = star.LastName
    };

    public static IReadOnlyCollection<StarViewModel> ToViewModels(this IReadOnlyCollection<Star> stars) =>
        stars.Select(ToViewModel).ToList();
}
