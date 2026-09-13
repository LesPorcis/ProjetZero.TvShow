using ProjectZero.TvShows.Application.Commands;
using ProjectZero.TvShows.Application.Exceptions;
using ProjectZero.TvShows.Application.Ports.In;
using ProjectZero.TvShows.Application.Ports.Out;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.UseCases;

internal sealed class TvShowsCatalog(ITvShowsRepository tvShowsRepository) : ITvShowsCatalog
{
    private const int MaxNameLength = 256;

    public Task<IReadOnlyCollection<TvShow>> ListAsync(CancellationToken cancellationToken = default)
    {
        return tvShowsRepository.GetAllAsync(cancellationToken);
    }

    public async Task<TvShow> GetByIdAsync(TvShowId id, CancellationToken cancellationToken = default)
    {
        var tvShow = await tvShowsRepository.FindByIdAsync(id, cancellationToken);

        return tvShow ?? throw new TvShowNotFoundException(id);
    }

    public async Task<TvShow> UpdateAsync(
        TvShowId id,
        UpdateTvShowCommand command,
        CancellationToken cancellationToken = default)
    {
        EnsureUpdateCommandIsValid(command);
        EnsureAllIdsArePositive(command.DirectorIds, nameof(command.DirectorIds));
        EnsureAllIdsArePositive(command.WriterIds, nameof(command.WriterIds));
        EnsureAllIdsArePositive(command.StarIds, nameof(command.StarIds));
        EnsureAllIdsArePositive(command.GenreIds, nameof(command.GenreIds));

        var tvShow = await tvShowsRepository.UpdateAsync(id, command, cancellationToken);
        return tvShow ?? throw new TvShowNotFoundException(id);
    }

    private static void EnsureUpdateCommandIsValid(UpdateTvShowCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new InvalidTvShowUpdateException("Name is required.");
        }

        if (command.Name.Length > MaxNameLength)
        {
            throw new InvalidTvShowUpdateException($"Name must not exceed {MaxNameLength} characters.");
        }

        if (command.Seasons <= 0)
        {
            throw new InvalidTvShowUpdateException("Seasons must be positive.");
        }

        if (command.Episodes <= 0)
        {
            throw new InvalidTvShowUpdateException("Episodes must be positive.");
        }
    }

    private static void EnsureAllIdsArePositive(IReadOnlyCollection<int> ids, string fieldName)
    {
        if (ids.Any(id => id <= 0))
        {
            throw new InvalidTvShowUpdateException($"All values in {fieldName} must be positive.");
        }
    }
}
