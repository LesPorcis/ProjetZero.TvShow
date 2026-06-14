using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Daos;

internal sealed class WriterDao : IEntityTypeConfiguration<WriterDao>
{
    public int PersonId { get; init; }
    public int TvShowId { get; init; }

    public PersonDao Person { get; init; } = null!;
    public TvShowDao TvShow { get; init; } = null!;

    public void Configure(EntityTypeBuilder<WriterDao> builder)
    {
        builder.ToTable("Writer");

        builder.HasKey(writer => new { writer.PersonId, writer.TvShowId });

        builder.HasOne(writer => writer.Person)
            .WithMany(person => person.WriterRoles)
            .HasForeignKey(writer => writer.PersonId);

        builder.HasOne(writer => writer.TvShow)
            .WithMany(tvShow => tvShow.Writers)
            .HasForeignKey(writer => writer.TvShowId);
    }
}
