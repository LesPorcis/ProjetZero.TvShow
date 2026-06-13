using ProjectZero.TvShows.Application.Ports.In;
using ProjectZero.TvShows.Application.Ports.Out;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.UseCases.ListTvShows;

internal sealed class ListTvShowsUseCase(ITvShowRepository repository) : IListTvShowsUseCase
{
    public Task<IReadOnlyCollection<TvShow>> ExecuteAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);
}
