namespace ProjectZero.TvShows.Domain;

public sealed class Director(int id, string firstName, string lastName)
{
    public int Id { get; } = id;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
}
