using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.Ports.In;

public interface ITvShowsCatalog
{
    Task<IReadOnlyCollection<TvShow>> ListAsync(CancellationToken cancellationToken = default);

    /// <exception cref="TvShowNotFoundException">
    /// Thrown when no TV show exists for the provided id.
    /// </exception>
    Task<TvShow> GetByIdAsync(TvShowId id, CancellationToken cancellationToken = default);
}
