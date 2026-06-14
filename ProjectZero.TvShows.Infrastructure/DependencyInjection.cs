using Microsoft.Extensions.DependencyInjection;
using ProjectZero.TvShows.Application.Ports.Out;
using ProjectZero.TvShows.Infrastructure.Repositories;

namespace ProjectZero.TvShows.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services) => 
        services
            .AddScoped<ITvShowsRepository, TvShowsRepository>();
}
