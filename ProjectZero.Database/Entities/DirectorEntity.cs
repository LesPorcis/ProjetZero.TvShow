using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Entities;

internal sealed class DirectorEntity : IEntityTypeConfiguration<DirectorEntity>
{
    public int PersonId { get; init; }
    public int TvShowId { get; init; }

    public PersonEntity Person { get; init; } = null!;
    public TvShowEntity TvShow { get; init; } = null!;

    public void Configure(EntityTypeBuilder<DirectorEntity> builder)
    {
        builder.ToTable("Director");

        builder.HasKey(director => new { director.PersonId, director.TvShowId });

        builder.HasOne(director => director.Person)
            .WithMany(person => person.DirectorRoles)
            .HasForeignKey(director => director.PersonId);

        builder.HasOne(director => director.TvShow)
            .WithMany(tvShow => tvShow.Directors)
            .HasForeignKey(director => director.TvShowId);
    }
}
