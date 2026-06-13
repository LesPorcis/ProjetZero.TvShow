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
    
    public List<DirectorDao> Directors { get; init; } = new List<DirectorDao>();
    public List<WriterDao> Writers { get; init; } = new List<WriterDao>();
    public List<StarDao> Stars { get; init; } = new List<StarDao>();
    public List<GenreDao> Genres { get; init; } = new List<GenreDao>();
    
    public void Configure(EntityTypeBuilder<TvShowDao> builder)
    {
        builder.HasKey(tvShow => tvShow.Id);

        builder.Property(tvShow => tvShow.Name)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.HasMany(tvShow => tvShow.Directors).WithMany(director => director.TvShows)
            .UsingEntity(join => join.ToTable("TvShowDirectors"));
        builder.HasMany(tvShow => tvShow.Writers).WithMany(writer => writer.TvShows)
            .UsingEntity(join => join.ToTable("TvShowWriters"));
        builder.HasMany(tvShow => tvShow.Stars).WithMany(star => star.TvShows)
            .UsingEntity(join => join.ToTable("TvShowStars"));
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
