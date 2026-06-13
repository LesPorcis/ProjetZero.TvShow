using TvShow.Domain;

namespace TvShow.Application.Ports.In;

public interface IListTvShowsUseCase
{
    Task<IReadOnlyCollection<Series>> ExecuteAsync(CancellationToken cancellationToken = default);
}
