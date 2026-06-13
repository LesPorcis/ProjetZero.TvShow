using Microsoft.EntityFrameworkCore;
using ProjectZero.Database.Daos;

namespace ProjectZero.Database;

/// <summary>
/// Contexte EF Core de la persistance des séries. Interne au projet base de données :
/// seul l'adaptateur d'infrastructure y accède (InternalsVisibleTo).
/// </summary>
internal sealed class TvShowDbContext : DbContext
{
    // Constructeur public requis par EF Core pour l'activation, même si la classe est internal.
    public TvShowDbContext(DbContextOptions<TvShowDbContext> options) : base(options)
    {
    }

    public DbSet<TvShowDao> TvShows => Set<TvShowDao>();
    public DbSet<DirectorDao> Directors => Set<DirectorDao>();
    public DbSet<WriterDao> Writers => Set<WriterDao>();
    public DbSet<StarDao> Stars => Set<StarDao>();
    public DbSet<GenreDao> Genres => Set<GenreDao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Charge toutes les IEntityTypeConfiguration<> portées par les DAO de l'assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TvShowDbContext).Assembly);
    }
}
