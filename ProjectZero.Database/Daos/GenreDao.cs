using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Daos;

/// <summary>
/// Entité de persistance des genres. Interne au projet base de données.
/// </summary>
internal sealed class GenreDao : IEntityTypeConfiguration<GenreDao>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<TvShowDao> TvShows { get; set; } = new List<TvShowDao>();

    public void Configure(EntityTypeBuilder<GenreDao> builder)
    {
        builder.HasKey(genre => genre.Id);

        builder.Property(genre => genre.Name)
            .HasMaxLength(128)
            .IsRequired();

        // Unicité du nom de genre.
        builder.HasIndex(genre => genre.Name).IsUnique();

        builder.Property(genre => genre.Description)
            .HasMaxLength(1024);
    }
}
