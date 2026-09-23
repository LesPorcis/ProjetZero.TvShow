using ProjectZero.TvShows.Application.Commands;
using ProjectZero.TvShows.Application.Exceptions;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.Ports.In;

public interface ITvShowsCatalog
{
    /// <summary>
    /// Retrieves all TV shows.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<TvShow>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the TV show identified by <paramref name="id"/>.
    /// </summary>
    /// <exception cref="NotFoundException">
    /// Thrown when no TV show exists for the provided id.
    /// </exception>
    Task<TvShow> GetByIdAsync(TvShowId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the TV show identified by <paramref name="id"/>.
    /// </summary>
    /// <exception cref="NotFoundException">
    /// Thrown when the TV show or a related entity referenced by the command does not exist.
    /// </exception>
    /// <exception cref="InvalidTvShowUpdateException">
    /// Thrown when the update command contains invalid values.
    /// </exception>
    Task<TvShow> UpdateAsync(
        TvShowId id,
        UpdateTvShowCommand command,
        CancellationToken cancellationToken = default
    );
}
