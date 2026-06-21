using ProjectZero.TvShows.Api.ViewModels;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Api.Mappers;

internal static class DirectorMapper
{
    public static DirectorViewModel ToViewModel(this Director director) => new()
    {
        Id = director.Id,
        FirstName = director.FirstName,
        LastName = director.LastName
    };

    public static IReadOnlyCollection<DirectorViewModel> ToViewModels(this IReadOnlyCollection<Director> directors) =>
        directors.Select(ToViewModel).ToList();
}
