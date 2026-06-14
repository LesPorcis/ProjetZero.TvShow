using ProjectZero.TvShows.Api.ViewModels;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Api.Mappers;

internal static class GenreMapper
{
    public static GenreViewModel ToViewModel(this Genre genre) => new()
    {
        Id = genre.Id,
        Name = genre.Name,
        Description = genre.Description
    };

    public static IReadOnlyCollection<GenreViewModel> ToViewModels(this IReadOnlyCollection<Genre> genres) =>
        genres.Select(ToViewModel).ToList();
}
