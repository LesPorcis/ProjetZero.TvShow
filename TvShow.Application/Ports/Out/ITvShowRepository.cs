using TvShowEntity = TvShow.Domain.TvShow;

namespace TvShow.Application.Ports.Out;

public interface ITvShowRepository
{
    Task<IReadOnlyCollection<TvShowEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}
