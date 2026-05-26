using TvShow.Application.Models;

namespace TvShow.Application.Ports.In;

public interface IListTvShowsUseCase
{
    Task<IReadOnlyList<TvShowResponse>> ExecuteAsync(CancellationToken cancellationToken = default);
}