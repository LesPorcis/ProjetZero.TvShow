using System.ComponentModel.DataAnnotations;

namespace ProjectZero.TvShows.Api.Requests;

public sealed class UpdateTvShowRequest : IValidatableObject
{
    [Required]
    [MaxLength(256)]
    public required string Name { get; init; }

    public DateOnly? ReleasedAt { get; init; }

    [Range(1, int.MaxValue)]
    public required int Seasons { get; init; }

    [Range(1, int.MaxValue)]
    public required int Episodes { get; init; }

    public required IReadOnlyCollection<int> DirectorIds { get; init; }
    public required IReadOnlyCollection<int> WriterIds { get; init; }
    public required IReadOnlyCollection<int> StarIds { get; init; }
    public required IReadOnlyCollection<int> GenreIds { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var (fieldName, ids) in new (string FieldName, IReadOnlyCollection<int> Ids)[]
                 {
                     (nameof(DirectorIds), DirectorIds),
                     (nameof(WriterIds), WriterIds),
                     (nameof(StarIds), StarIds),
                     (nameof(GenreIds), GenreIds)
                 })
        {
            if (ids.Any(id => id <= 0))
            {
                yield return new ValidationResult(
                    $"All values in {fieldName} must be positive.",
                    [fieldName]);
            }
        }
    }
}
