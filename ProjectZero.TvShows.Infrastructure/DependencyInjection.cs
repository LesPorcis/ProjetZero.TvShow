using Microsoft.Extensions.DependencyInjection;
using ProjectZero.TvShows.Application.Ports.Out;
using ProjectZero.TvShows.Infrastructure.Repositories;

namespace ProjectZero.TvShows.Infrastructure;

/// <summary>
/// Méthode d'extension pour ajouter les services d'infrastructure
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITvShowRepository, InMemoryTvShowRepository>();

        return services;
    }
}
