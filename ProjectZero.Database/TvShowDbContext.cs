using Microsoft.EntityFrameworkCore;
using ProjectZero.Database.Daos;

namespace ProjectZero.Database;

internal sealed class TvShowDbContext : DbContext
{
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TvShowDbContext).Assembly);
    }
}
