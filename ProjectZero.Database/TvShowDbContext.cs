using Microsoft.EntityFrameworkCore;

namespace ProjectZero.Database;

internal sealed class TvShowDbContext(DbContextOptions<TvShowDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TvShowDbContext).Assembly);
    }
}
