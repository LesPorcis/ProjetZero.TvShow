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

    public Task<TvShow> CreateAsync(
        CreateTvShowCommand command,
        CancellationToken cancellationToken = default)
    {
        var name = command.Name?.Trim() ?? string.Empty;
        Dictionary<string, string[]> errors = [];

        if (string.IsNullOrEmpty(name))
        {
            errors[nameof(command.Name)] = ["Name is required."];
        }
        else if (name.Length > MaxNameLength)
        {
            errors[nameof(command.Name)] = [$"Name must not exceed {MaxNameLength} characters."];
        }

        if (command.Seasons < 0)
        {
            errors[nameof(command.Seasons)] = ["Seasons must be greater than or equal to zero."];
        }

        if (command.Episodes < 0)
        {
            errors[nameof(command.Episodes)] = ["Episodes must be greater than or equal to zero."];
        }
        else if (command.Seasons == 0 && command.Episodes > 0)
        {
            errors[nameof(command.Episodes)] = ["Episodes must be zero when seasons is zero."];
        }

        if (errors.Count > 0)
        {
            throw new InvalidTvShowException(errors);
        }

        var tvShowToCreate = new TvShowToCreate
        {
            Name = name,
            ReleasedAt = command.ReleasedAt,
            Seasons = command.Seasons,
            Episodes = command.Episodes
        };

        return tvShowsRepository.CreateAsync(tvShowToCreate, cancellationToken);
    }
}
