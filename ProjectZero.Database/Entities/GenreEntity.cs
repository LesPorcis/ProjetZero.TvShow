using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Entities;

internal sealed class GenreEntity : IEntityTypeConfiguration<GenreEntity>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public ICollection<TvShowEntity> TvShows { get; init; } = new List<TvShowEntity>();

    public void Configure(EntityTypeBuilder<GenreEntity> builder)
    {
        builder.ToTable("Genre");

        builder.HasKey(genre => genre.Id);

        builder.Property(genre => genre.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(genre => genre.Name).IsUnique();

        builder.Property(genre => genre.Description)
            .HasMaxLength(1024);
    }
}
