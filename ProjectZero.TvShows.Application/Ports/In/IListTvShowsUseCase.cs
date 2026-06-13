using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.Ports.In;

public interface IListTvShowsUseCase
{
    Task<IReadOnlyCollection<TvShow>> ExecuteAsync(CancellationToken cancellationToken = default);
}
