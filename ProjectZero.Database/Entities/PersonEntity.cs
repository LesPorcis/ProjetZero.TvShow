using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Entities;

internal sealed class PersonEntity : IEntityTypeConfiguration<PersonEntity>
{
    public int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    public List<DirectorEntity> DirectorRoles { get; init; } = [];
    public List<WriterEntity> WriterRoles { get; init; } = [];
    public List<StarEntity> StarRoles { get; init; } = [];

    public void Configure(EntityTypeBuilder<PersonEntity> builder)
    {
        builder.ToTable("Persons");

        builder.HasKey(person => person.Id);

        builder.Property(person => person.FirstName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(person => person.LastName)
            .HasMaxLength(128)
            .IsRequired();
    }
}
