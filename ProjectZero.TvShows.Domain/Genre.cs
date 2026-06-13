namespace ProjectZero.TvShows.Domain;

public sealed class Genre
{
    public Genre(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public int Id { get; }
    public string Name { get; }
    public string Description { get; }
}
