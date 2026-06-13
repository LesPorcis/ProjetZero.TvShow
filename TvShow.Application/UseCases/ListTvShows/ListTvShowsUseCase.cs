using TvShow.Application.Ports.In;
using TvShow.Application.Ports.Out;

namespace TvShow.Application.UseCases.ListTvShows;

internal sealed class ListTvShowsUseCase(ITvShowRepository repository) : IListTvShowsUseCase
{
    public Task<IReadOnlyCollection<TvShow.Domain.TvShow>> ExecuteAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);
}
