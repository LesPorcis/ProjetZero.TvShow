using ProjectZero.TvShows.Application.Commands;
using ProjectZero.TvShows.Application.Exceptions;
using ProjectZero.TvShows.Application.Ports.In;
using ProjectZero.TvShows.Application.Ports.Out;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Application.UseCases;

internal sealed class TvShowsCatalog(ITvShowsRepository tvShowsRepository) : ITvShowsCatalog
{
    private const int MaxNameLength = 256;

    public Task<IReadOnlyCollection<TvShow>> ListAsync(
        CancellationToken cancellationToken = default
    )
    {
        return tvShowsRepository.GetAllAsync(cancellationToken);
    }

    public async Task<TvShow> GetByIdAsync(
        TvShowId id,
        CancellationToken cancellationToken = default
    )
    {
        var tvShow = await tvShowsRepository.FindByIdAsync(id, cancellationToken);

        return tvShow ?? throw new NotFoundException(id);
    }

    public async Task<TvShow> UpdateAsync(
        TvShowId id,
        UpdateTvShowCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var name = ValidateUpdateCommand(command);
        var validatedCommand = command with { Name = name };

        await EnsureAllRelatedEntitiesExistAsync(validatedCommand, cancellationToken);

        var tvShow = await tvShowsRepository.UpdateAsync(
            id,
            validatedCommand,
            cancellationToken
        );

        return tvShow ?? throw new NotFoundException(id);
    }

    private async Task EnsureAllRelatedEntitiesExistAsync(
        UpdateTvShowCommand command,
        CancellationToken cancellationToken)
    {
        await EnsureAllPersonsExistAsync("Director", command.DirectorIds, cancellationToken);
        await EnsureAllPersonsExistAsync("Writer", command.WriterIds, cancellationToken);
        await EnsureAllPersonsExistAsync("Star", command.StarIds, cancellationToken);
        await EnsureAllGenresExistAsync(command.GenreIds, cancellationToken);
    }

    private async Task EnsureAllGenresExistAsync(
        IReadOnlyCollection<int> genreIds,
        CancellationToken cancellationToken)
    {
        var distinctIds = genreIds.Distinct().ToList();
        if (distinctIds.Count == 0)
        {
            return;
        }

        var foundIds = await tvShowsRepository.FindExistingGenreIdsAsync(
            distinctIds,
            cancellationToken
        );

        ThrowIfAnyMissing("Genre", distinctIds, foundIds);
    }

    private async Task EnsureAllPersonsExistAsync(
        string entityName,
        IReadOnlyCollection<int> personIds,
        CancellationToken cancellationToken)
    {
        var distinctIds = personIds.Distinct().ToList();
        if (distinctIds.Count == 0)
        {
            return;
        }

        var foundIds = await tvShowsRepository.FindExistingPersonIdsAsync(
            distinctIds,
            cancellationToken
        );

        ThrowIfAnyMissing(entityName, distinctIds, foundIds);
    }

    private static void ThrowIfAnyMissing(
        string entityName,
        IReadOnlyCollection<int> requestedIds,
        IReadOnlyCollection<int> foundIds)
    {
        var missingIds = requestedIds.Except(foundIds).ToList();
        if (missingIds.Count > 0)
        {
            throw new NotFoundException(entityName, missingIds);
        }
    }

    private static string ValidateUpdateCommand(UpdateTvShowCommand command)
    {
        var name = command.Name.Trim();
        Dictionary<string, string[]> errors = [];

        if (string.IsNullOrEmpty(name))
        {
            errors[nameof(command.Name)] = ["Name is required."];
        }
        else if (name.Length > MaxNameLength)
        {
            errors[nameof(command.Name)] = [$"Name must not exceed {MaxNameLength} characters."];
        }

        if (command.Seasons <= 0)
        {
            errors[nameof(command.Seasons)] = ["Seasons must be positive."];
        }

        if (command.Episodes <= 0)
        {
            errors[nameof(command.Episodes)] = ["Episodes must be positive."];
        }

        foreach (
            var (fieldName, ids) in new (string FieldName, IReadOnlyCollection<int> Ids)[]
            {
                (nameof(command.DirectorIds), command.DirectorIds),
                (nameof(command.WriterIds), command.WriterIds),
                (nameof(command.StarIds), command.StarIds),
                (nameof(command.GenreIds), command.GenreIds),
            }
        )
        {
            if (ids.Any(relatedId => relatedId <= 0))
            {
                errors[fieldName] = [$"All values in {fieldName} must be positive."];
            }
            else if (ids.Count != ids.Distinct().Count())
            {
                errors[fieldName] = [$"All values in {fieldName} must be unique."];
            }
        }

        if (errors.Count > 0)
        {
            throw new InvalidTvShowUpdateException(errors);
        }

        return name;
    }
}
