namespace TvShow.Application.Ports.In;

public interface IListTvShowsUseCase
{
    Task<IReadOnlyCollection<TvShow.Domain.TvShow>> ExecuteAsync(CancellationToken cancellationToken = default);
}
