using TvShow.Application.Models;
using TvShow.Application.Ports.In;
using TvShow.Application.Ports.Out;

namespace TvShow.Application.UseCases.ListTvShows;

internal sealed class ListTvShowsUseCase : IListTvShowsUseCase
{
    private readonly ITvShowRepository _repository;

    public ListTvShowsUseCase(ITvShowRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TvShowResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var tvShows = await _repository.GetAllAsync();

        return tvShows
            .Select(tvShow => new TvShowResponse(
                tvShow.Id,
                tvShow.Name,
                tvShow.ReleasedAt,
                tvShow.Seasons,
                tvShow.Episodes
            ))
            .ToList();
    }
}