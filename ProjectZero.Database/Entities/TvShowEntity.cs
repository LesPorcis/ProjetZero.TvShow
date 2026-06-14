using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Entities;

internal sealed class TvShowEntity : IEntityTypeConfiguration<TvShowEntity>
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public DateOnly? ReleasedAt { get; init; }
    public int Seasons { get; init; }
    public int Episodes { get; init; }

    public List<DirectorEntity> Directors { get; init; } = [];
    public List<WriterEntity> Writers { get; init; } = [];
    public List<StarEntity> Stars { get; init; } = [];
    public List<GenreEntity> Genres { get; init; } = [];

    public void Configure(EntityTypeBuilder<TvShowEntity> builder)
    {
        builder.ToTable("TvShows");

        builder.HasKey(tvShow => tvShow.Id);

        builder.Property(tvShow => tvShow.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasMany(tvShow => tvShow.Genres).WithMany(genre => genre.TvShows)
            .UsingEntity(join => join.ToTable("TvShowGenres"));

        builder.HasData(
            new TvShowEntity
            {
                Id = 1,
                Name = "Breaking Bad",
                ReleasedAt = new DateOnly(2008,
                    1,
                    20),
                Seasons = 5,
                Episodes = 62
            },
            new TvShowEntity
            {
                Id = 2,
                Name = "The Last of Us",
                ReleasedAt = new DateOnly(2023,
                    1,
                    15),
                Seasons = 2,
                Episodes = 16
            });
    }
}
