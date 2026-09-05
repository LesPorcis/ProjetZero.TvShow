using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.Ports.In;

public interface ITvShowsCreation
{
    Task<TvShow> CreateAsync(
        CreateTvShowCommand command,
        CancellationToken cancellationToken = default);
}
