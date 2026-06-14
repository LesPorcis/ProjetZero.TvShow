using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Entities;

internal sealed class PersonEntity : IEntityTypeConfiguration<PersonEntity>
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;

    public ICollection<DirectorEntity> DirectorRoles { get; init; } = new List<DirectorEntity>();
    public ICollection<WriterEntity> WriterRoles { get; init; } = new List<WriterEntity>();
    public ICollection<StarEntity> StarRoles { get; init; } = new List<StarEntity>();

    public void Configure(EntityTypeBuilder<PersonEntity> builder)
    {
        builder.ToTable("Person");

        builder.HasKey(person => person.Id);

        builder.Property(person => person.FirstName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(person => person.LastName)
            .HasMaxLength(128)
            .IsRequired();
    }
}
