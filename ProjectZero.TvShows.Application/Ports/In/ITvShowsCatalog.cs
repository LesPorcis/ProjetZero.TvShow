using ProjectZero.TvShows.Application.Commands;
using ProjectZero.TvShows.Application.Exceptions;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.Ports.In;

public interface ITvShowsCatalog
{
    Task<IReadOnlyCollection<TvShow>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the TV show identified by <paramref name="id"/>.
    /// </summary>
    /// <exception cref="TvShowNotFoundException">
    /// Thrown when no TV show exists for the provided id.
    /// </exception>
    Task<TvShow> GetByIdAsync(TvShowId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the TV show identified by <paramref name="id"/>.
    /// </summary>
    /// <exception cref="TvShowNotFoundException">
    /// Thrown when no TV show exists for the provided id.
    /// </exception>
    /// <exception cref="InvalidTvShowUpdateException">
    /// Thrown when the update command contains invalid values.
    /// </exception>
    /// <exception cref="RelatedEntityNotFoundException">
    /// Thrown when one or more related entity ids do not exist.
    /// </exception>
    Task<TvShow> UpdateAsync(
        TvShowId id,
        UpdateTvShowCommand command,
        CancellationToken cancellationToken = default);
}
