using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectZero.Database;

public static class DependencyInjection
{
    private const string ConnectionStringName = "TvShowDb";

    public static IServiceCollection AddEfPostgreSql
    (
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        services.AddDbContext<TvShowDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}
