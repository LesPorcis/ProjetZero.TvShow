using ProjectZero.TvShows.Application.Ports.In;
using ProjectZero.TvShows.Application.Ports.Out;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.UseCases;

internal sealed class TvShowsCatalog(ITvShowsRepository tvShowsRepository) : ITvShowsCatalog
{
    public Task<IReadOnlyCollection<TvShow>> ListAsync(CancellationToken cancellationToken = default)
    {
        return tvShowsRepository.GetAllAsync(cancellationToken);
    }
}
