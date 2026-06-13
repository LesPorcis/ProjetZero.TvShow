namespace TvShow.Application.Ports.Out;

public interface ITvShowRepository
{
    Task<IReadOnlyCollection<TvShow.Domain.TvShow>> GetAllAsync(CancellationToken cancellationToken = default);
}
