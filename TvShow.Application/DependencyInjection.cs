using Microsoft.Extensions.DependencyInjection;
using TvShow.Application.Ports.In;
using TvShow.Application.UseCases.ListTvShows;

namespace TvShow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IListTvShowsUseCase, ListTvShowsUseCase>();

        return services;
    }
}
