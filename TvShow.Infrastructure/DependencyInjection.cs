using Microsoft.Extensions.DependencyInjection;
using TvShow.Application.Ports.Out;
using TvShow.Infrastructure.Repositories;

namespace TvShow.Infrastructure;

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
