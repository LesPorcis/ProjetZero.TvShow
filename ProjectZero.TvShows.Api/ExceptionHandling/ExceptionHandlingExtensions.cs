using Microsoft.Extensions.DependencyInjection;

namespace ProjectZero.TvShows.Api.ExceptionHandling;

internal static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services) =>
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
            // so it must be registered from within this project (the Web host cannot see it).
            .AddExceptionHandler<BusinessExceptionHandler>();
}
