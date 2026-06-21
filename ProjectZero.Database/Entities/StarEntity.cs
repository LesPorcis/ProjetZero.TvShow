using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Entities;

internal sealed class StarEntity : IEntityTypeConfiguration<StarEntity>
{
    public int PersonId { get; init; }
    public int TvShowId { get; init; }

    public PersonEntity Person { get; init; } = null!;
    public TvShowEntity TvShow { get; init; } = null!;

    public void Configure(EntityTypeBuilder<StarEntity> builder)
    {
        builder.ToTable("Stars");

        builder.HasKey(star => new { star.PersonId, star.TvShowId });

        builder.HasOne(star => star.Person)
            .WithMany(person => person.StarRoles)
            .HasForeignKey(star => star.PersonId);

        builder.HasOne(star => star.TvShow)
            .WithMany(tvShow => tvShow.Stars)
            .HasForeignKey(star => star.TvShowId);
    }
}
