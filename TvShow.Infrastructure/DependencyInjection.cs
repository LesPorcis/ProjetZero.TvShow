using Microsoft.Extensions.DependencyInjection;
using TvShow.Application.Ports;
using TvShow.Infrastructure.Repositories;

namespace TvShow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITvShowRepository, InMemoryTvShowRepository>();

        return services;
    }
}
