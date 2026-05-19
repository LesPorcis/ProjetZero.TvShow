using Microsoft.Extensions.DependencyInjection;
using TvShow.Application.Services;

namespace TvShow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<TvShowManager>();

        return services;
    }
}
