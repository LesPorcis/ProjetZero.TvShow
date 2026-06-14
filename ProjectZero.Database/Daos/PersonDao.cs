using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectZero.Database.Daos;

internal sealed class PersonDao : IEntityTypeConfiguration<PersonDao>
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;

    public ICollection<DirectorDao> DirectorRoles { get; init; } = new List<DirectorDao>();
    public ICollection<WriterDao> WriterRoles { get; init; } = new List<WriterDao>();
    public ICollection<StarDao> StarRoles { get; init; } = new List<StarDao>();

    public void Configure(EntityTypeBuilder<PersonDao> builder)
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
