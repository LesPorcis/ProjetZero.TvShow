using TvShow.Application.Ports.In;
using TvShow.Application.Ports.Out;
using TvShow.Domain;

namespace TvShow.Application.UseCases.ListTvShows;

internal sealed class ListTvShowsUseCase(ITvShowRepository repository) : IListTvShowsUseCase
{
    public Task<IReadOnlyCollection<Series>> ExecuteAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);
}
