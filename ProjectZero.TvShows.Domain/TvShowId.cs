namespace ProjectZero.TvShows.Domain;

public readonly record struct TvShowId
{
    public int Value { get; }

    public TvShowId(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "TV show id must be positive.");
        }

        Value = value;
    }
}
