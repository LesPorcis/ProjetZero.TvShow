using ProjectZero.TvShows.Api.Requests;
using ProjectZero.TvShows.Application.Commands;

namespace ProjectZero.TvShows.Api.Mappers;

internal static class UpdateTvShowRequestMapper
{
    public static UpdateTvShowCommand ToCommand(this UpdateTvShowInput request) => new(
        request.Name,
        request.ReleasedAt,
        request.Seasons,
        request.Episodes,
        request.DirectorIds,
        request.WriterIds,
        request.StarIds,
        request.GenreIds);
}