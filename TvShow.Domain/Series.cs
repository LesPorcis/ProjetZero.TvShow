namespace TvShow.Domain;

/// <summary>
/// Entité de domaine représentant une série télévisée.
/// Immuable : propriétés en lecture seule, valeurs fixées à la construction.
/// </summary>
public sealed class Series
{
    public Series(
        int id,
        string name,
        DateOnly? releasedAt,
        int seasons,
        int episodes,
        IReadOnlyCollection<Director>? directors = null,
        IReadOnlyCollection<Writer>? writers = null,
        IReadOnlyCollection<Star>? stars = null,
        IReadOnlyCollection<Genre>? genres = null)
    {
        Id = id;
        Name = name;
        ReleasedAt = releasedAt;
        Seasons = seasons;
        Episodes = episodes;
        Directors = directors is null ? [] : [.. directors];
        Writers = writers is null ? [] : [.. writers];
        Stars = stars is null ? [] : [.. stars];
        Genres = genres is null ? [] : [.. genres];
    }

    public int Id { get; }
    public string Name { get; }
    public DateOnly? ReleasedAt { get; }
    public int Seasons { get; }
    public int Episodes { get; }

    public IReadOnlyCollection<Director> Directors { get; }
    public IReadOnlyCollection<Writer> Writers { get; }
    public IReadOnlyCollection<Star> Stars { get; }
    public IReadOnlyCollection<Genre> Genres { get; }
}
