using TvShowEntity = TvShow.Domain.TvShow;

namespace TvShow.Application.Ports;

public interface ITvShowRepository
{
    Task<IReadOnlyCollection<TvShowEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}
