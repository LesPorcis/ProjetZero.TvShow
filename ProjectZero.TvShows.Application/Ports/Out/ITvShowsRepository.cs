using ProjectZero.TvShows.Application.Commands;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.Ports.Out;

public interface ITvShowsRepository
{
    Task<IReadOnlyCollection<TvShow>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TvShow?> FindByIdAsync(TvShowId id, CancellationToken cancellationToken = default);
    Task<TvShow?> UpdateAsync(
        TvShowId id,
        UpdateTvShowCommand command,
        CancellationToken cancellationToken = default);
}
