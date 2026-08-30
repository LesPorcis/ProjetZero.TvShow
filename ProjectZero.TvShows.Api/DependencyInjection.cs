using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ProjectZero.TvShows.Api.Filters;
using ProjectZero.TvShows.Application;
using ProjectZero.TvShows.Infrastructure;

namespace ProjectZero.TvShows.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddTvShowsModule(this IServiceCollection services) =>
        services
            .Configure<MvcOptions>(options =>
            {
                options.Filters.Add<TvShowsExceptionFilter>();
            })
            .AddApplication()
            .AddInfrastructure();
}
