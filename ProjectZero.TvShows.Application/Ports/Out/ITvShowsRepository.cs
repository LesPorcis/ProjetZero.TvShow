using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.Ports.Out;

public interface ITvShowsRepository
{
    Task<IReadOnlyCollection<TvShow>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TvShow?> FindByIdAsync(TvShowId id, CancellationToken cancellationToken = default);

    Task<TvShow> CreateAsync(
        string name,
        DateOnly? releasedAt,
        int seasons,
        int episodes,
        CancellationToken cancellationToken = default);
}
