using Microsoft.Extensions.DependencyInjection;
using ProjectZero.TvShows.Application.Ports.In;
using ProjectZero.TvShows.Application.UseCases;

namespace ProjectZero.TvShows.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services) =>
        services
            .AddScoped<ITvShowsCatalog, TvShowsCatalog>()
            .AddScoped<ITvShowsCreation, TvShowsCreation>();
}
