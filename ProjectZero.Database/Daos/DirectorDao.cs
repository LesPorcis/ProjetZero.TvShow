using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Daos;

/// <summary>
/// Entité de persistance des réalisateurs. Interne au projet base de données.
/// </summary>
internal sealed class DirectorDao : IEntityTypeConfiguration<DirectorDao>
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public ICollection<TvShowDao> TvShows { get; set; } = new List<TvShowDao>();

    public void Configure(EntityTypeBuilder<DirectorDao> builder)
    {
        builder.HasKey(director => director.Id);

        builder.Property(director => director.FirstName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(director => director.LastName)
            .HasMaxLength(128)
            .IsRequired();
    }
}
