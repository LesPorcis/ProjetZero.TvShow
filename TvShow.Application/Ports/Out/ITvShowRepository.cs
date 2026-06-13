using TvShow.Domain;

namespace TvShow.Application.Ports.Out;

public interface ITvShowRepository
{
    Task<IReadOnlyCollection<Series>> GetAllAsync(CancellationToken cancellationToken = default);
}
