namespace TvShow.Application.Models;

public sealed record TvShowResponse(
    int Id,
    string Name,
    DateOnly? ReleasedAt,
    int Seasons,
    int Episodes);
