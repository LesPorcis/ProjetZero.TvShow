using Microsoft.Extensions.DependencyInjection;
using ProjectZero.TvShows.Application.Ports.In;
using ProjectZero.TvShows.Application.UseCases.ListTvShows;

namespace ProjectZero.TvShows.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IListTvShowsUseCase, ListTvShowsUseCase>();

        return services;
    }
}
