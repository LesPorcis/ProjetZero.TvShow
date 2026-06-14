using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Daos;

internal sealed class DirectorDao : IEntityTypeConfiguration<DirectorDao>
{
    public int PersonId { get; init; }
    public int TvShowId { get; init; }

    public PersonDao Person { get; init; } = null!;
    public TvShowDao TvShow { get; init; } = null!;

    public void Configure(EntityTypeBuilder<DirectorDao> builder)
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
