using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Daos;

internal sealed class StarDao : IEntityTypeConfiguration<StarDao>
{
    public int PersonId { get; init; }
    public int TvShowId { get; init; }

    public PersonDao Person { get; init; } = null!;
    public TvShowDao TvShow { get; init; } = null!;

    public void Configure(EntityTypeBuilder<StarDao> builder)
    {
        builder.ToTable("Star");

        builder.HasKey(star => new { star.PersonId, star.TvShowId });

        builder.HasOne(star => star.Person)
            .WithMany(person => person.StarRoles)
            .HasForeignKey(star => star.PersonId);

        builder.HasOne(star => star.TvShow)
            .WithMany(tvShow => tvShow.Stars)
            .HasForeignKey(star => star.TvShowId);
    }
}
