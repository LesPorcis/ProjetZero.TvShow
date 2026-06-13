using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Daos;

/// <summary>
/// Entité de persistance des acteurs. Interne au projet base de données.
/// </summary>
internal sealed class StarDao : IEntityTypeConfiguration<StarDao>
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public ICollection<TvShowDao> TvShows { get; set; } = new List<TvShowDao>();

    public void Configure(EntityTypeBuilder<StarDao> builder)
    {
        builder.HasKey(star => star.Id);

        builder.Property(star => star.FirstName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(star => star.LastName)
            .HasMaxLength(128)
            .IsRequired();
    }
}
