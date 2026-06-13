using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectZero.Database;

internal sealed class TvShowDbContextFactory : IDesignTimeDbContextFactory<TvShowDbContext>
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=tvshow;Username=postgres;Password=postgres";

    public TvShowDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__TvShowDb")
            ?? DefaultConnectionString;

        var options = new DbContextOptionsBuilder<TvShowDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new TvShowDbContext(options);
    }
}
