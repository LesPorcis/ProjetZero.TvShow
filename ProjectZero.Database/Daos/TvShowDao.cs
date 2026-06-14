using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Daos;

internal sealed class TvShowDao : IEntityTypeConfiguration<TvShowDao>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateOnly? ReleasedAt { get; init; }
    public int Seasons { get; init; }
    public int Episodes { get; init; }

    public ICollection<DirectorDao> Directors { get; init; } = new List<DirectorDao>();
    public ICollection<WriterDao> Writers { get; init; } = new List<WriterDao>();
    public ICollection<StarDao> Stars { get; init; } = new List<StarDao>();
    public ICollection<GenreDao> Genres { get; init; } = new List<GenreDao>();

    public void Configure(EntityTypeBuilder<TvShowDao> builder)
    {
        builder.ToTable("TvShow");

        builder.HasKey(tvShow => tvShow.Id);

        builder.Property(tvShow => tvShow.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasMany(tvShow => tvShow.Genres).WithMany(genre => genre.TvShows)
            .UsingEntity(join => join.ToTable("TvShowGenres"));

        builder.HasData(
            new TvShowDao
            {
                Id = 1,
                Name = "Breaking Bad",
                ReleasedAt = new DateOnly(2008, 1, 20),
                Seasons = 5,
                Episodes = 62
            },
            new TvShowDao
            {
                Id = 2,
                Name = "The Last of Us",
                ReleasedAt = new DateOnly(2023, 1, 15),
                Seasons = 2,
                Episodes = 16
            });
    }
}
