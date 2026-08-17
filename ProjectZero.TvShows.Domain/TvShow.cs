namespace ProjectZero.TvShows.Domain;

public sealed class TvShow
(
    TvShowId id,
    string name,
    DateOnly? releasedAt,
    int seasons,
    int episodes,
    IReadOnlyCollection<Director> directors,
    IReadOnlyCollection<Writer> writers,
    IReadOnlyCollection<Star> stars,
    IReadOnlyCollection<Genre> genres
)
{
    public TvShowId Id { get; } = id;
    public string Name { get; } = name;
    public DateOnly? ReleasedAt { get; } = releasedAt;
    public int Seasons { get; } = seasons;
    public int Episodes { get; } = episodes;

    public IReadOnlyCollection<Director> Directors { get; } = directors;
    public IReadOnlyCollection<Writer> Writers { get; } = writers;
    public IReadOnlyCollection<Star> Stars { get; } = stars;
    public IReadOnlyCollection<Genre> Genres { get; } = genres;
}
