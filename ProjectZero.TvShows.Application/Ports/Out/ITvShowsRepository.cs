using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.Ports.Out;

public interface ITvShowsRepository
{
    Task<IReadOnlyCollection<TvShow>> GetAllAsync(CancellationToken cancellationToken = default);
}
