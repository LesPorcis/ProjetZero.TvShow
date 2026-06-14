using ProjectZero.TvShows.Api.ViewModels;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Api.Mappers;

internal static class WriterMapper
{
    public static WriterViewModel ToViewModel(this Writer writer) => new()
    {
        Id = writer.Id,
        FirstName = writer.FirstName,
        LastName = writer.LastName
    };

    public static IReadOnlyCollection<WriterViewModel> ToViewModels(this IReadOnlyCollection<Writer> writers) =>
        writers.Select(ToViewModel).ToList();
}
