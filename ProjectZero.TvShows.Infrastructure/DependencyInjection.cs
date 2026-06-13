using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectZero.Database;
using ProjectZero.TvShows.Application.Ports.Out;
using ProjectZero.TvShows.Infrastructure.Repositories;

namespace ProjectZero.TvShows.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = configuration["Persistence:Provider"] ?? "InMemory";

        if (string.Equals(provider, "Postgres", StringComparison.OrdinalIgnoreCase))
        {
            var connectionString = configuration.GetConnectionString("TvShowDb");

            services.AddDbContext<TvShowDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<ITvShowsRepository, TvShowsRepository>();
        }
        else
        {
            services.AddSingleton<ITvShowsRepository, MockedTvShowsRepository>();
        }

        return services;
    }
}
