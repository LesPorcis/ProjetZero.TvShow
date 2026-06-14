using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Entities;

internal sealed class TvShowEntity : IEntityTypeConfiguration<TvShowEntity>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateOnly? ReleasedAt { get; init; }
    public int Seasons { get; init; }
    public int Episodes { get; init; }

    public ICollection<DirectorEntity> Directors { get; init; } = new List<DirectorEntity>();
    public ICollection<WriterEntity> Writers { get; init; } = new List<WriterEntity>();
    public ICollection<StarEntity> Stars { get; init; } = new List<StarEntity>();
    public ICollection<GenreEntity> Genres { get; init; } = new List<GenreEntity>();

    public void Configure(EntityTypeBuilder<TvShowEntity> builder)
    {
        builder.ToTable("TvShow");

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
                ReleasedAt = new DateOnly(2008, 1, 20),
                Seasons = 5,
                Episodes = 62
            },
            new TvShowEntity
            {
                Id = 2,
                Name = "The Last of Us",
                ReleasedAt = new DateOnly(2023, 1, 15),
                Seasons = 2,
                Episodes = 16
            });
    }
}
