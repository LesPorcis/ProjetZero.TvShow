using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Entities;

internal sealed class WriterEntity : IEntityTypeConfiguration<WriterEntity>
{
    public int PersonId { get; init; }
    public int TvShowId { get; init; }

    public PersonEntity Person { get; init; } = null!;
    public TvShowEntity TvShow { get; init; } = null!;

    public void Configure(EntityTypeBuilder<WriterEntity> builder)
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
