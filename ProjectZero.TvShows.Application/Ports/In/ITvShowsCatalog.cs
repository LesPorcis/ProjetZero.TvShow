using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.Ports.In;

public interface ITvShowsCatalog
{
    Task<IReadOnlyCollection<TvShow>> ListAsync(CancellationToken cancellationToken = default);
    Task<TvShow?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
