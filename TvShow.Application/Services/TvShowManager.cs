using TvShow.Application.Models;
using TvShow.Application.Ports;

namespace TvShow.Application.Services;

public sealed class TvShowManager(ITvShowRepository tvShowRepository)
{
    public async Task<IReadOnlyCollection<TvShowResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tvShows = await tvShowRepository.GetAllAsync(cancellationToken);

        return tvShows
            .Select(tvShow => new TvShowResponse(
                tvShow.Id,
                tvShow.Name,
                tvShow.ReleasedAt,
                tvShow.Seasons,
                tvShow.Episodes))
            .ToArray();
    }
}
