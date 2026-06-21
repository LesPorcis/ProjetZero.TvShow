namespace ProjectZero.TvShows.Domain;

public sealed class Genre(int id, string name, string description)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public string Description { get; } = description;
}
