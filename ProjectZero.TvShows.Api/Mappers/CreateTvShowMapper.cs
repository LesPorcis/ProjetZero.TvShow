using ProjectZero.TvShows.Api.Requests;
using ProjectZero.TvShows.Application.Ports.In;

namespace ProjectZero.TvShows.Api.Mappers;

internal static class CreateTvShowMapper
{
    public static CreateTvShowCommand ToCommand(this CreateTvShowRequest request) => new()
    {
        Name = request.Name,
        ReleasedAt = request.ReleasedAt,
        Seasons = request.Seasons,
        Episodes = request.Episodes
    };
}
