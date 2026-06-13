namespace ProjectZero.TvShows.Api.ViewModels;

/// <summary>
/// Contrat de sortie HTTP exposé par l'API pour une série.
/// Distinct du DTO applicatif : il appartient à la couche API.
/// </summary>
public sealed record TvShowViewModel(
    int Id,
    string Name,
    DateOnly? ReleasedAt,
    int Seasons,
    int Episodes);
