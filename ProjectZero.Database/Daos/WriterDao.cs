using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Daos;

internal sealed class WriterDao : IEntityTypeConfiguration<WriterDao>
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public ICollection<TvShowDao> TvShows { get; set; } = new List<TvShowDao>();

    public void Configure(EntityTypeBuilder<WriterDao> builder)
    {
        builder.HasKey(writer => writer.Id);

        builder.Property(writer => writer.FirstName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(writer => writer.LastName)
            .HasMaxLength(128)
            .IsRequired();
    }
}
