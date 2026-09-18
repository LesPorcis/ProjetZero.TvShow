using Microsoft.Extensions.DependencyInjection;
using ProjectZero.TvShows.Api.ExceptionHandling;
using ProjectZero.TvShows.Application;
using ProjectZero.TvShows.Infrastructure;

namespace ProjectZero.TvShows.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddTvShowsModule(this IServiceCollection services) =>
        services
            // Registers IProblemDetailsService + the problem+json writer, and enriches every
            // ProblemDetails (including the framework's default 500) with instance and trace id.
            .AddProblemDetails(options =>
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Instance ??= context.HttpContext.Request.Path;
                    context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                })
            // Handlers run in registration order, most specific first. The handler is internal,
            // so it must be registered here (the Web host cannot see it).
            .AddExceptionHandler<TvShowsCatalogExceptionHandler>()
            .AddApplication()
            .AddInfrastructure();
}
