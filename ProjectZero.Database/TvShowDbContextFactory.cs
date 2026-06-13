using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectZero.Database;

/// <summary>
/// Fabrique design-time utilisée par les outils EF Core (<c>dotnet ef migrations</c>).
/// Le runtime, lui, construit le contexte via <c>AddInfrastructure</c> + DI.
/// Interne : découverte par réflexion des outils EF, jamais exposée à l'application.
/// </summary>
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
