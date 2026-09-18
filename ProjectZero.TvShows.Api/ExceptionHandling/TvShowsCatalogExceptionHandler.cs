using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectZero.TvShows.Application.Exceptions;

namespace ProjectZero.TvShows.Api.ExceptionHandling;

internal sealed class TvShowsCatalogExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment,
    ILogger<TvShowsCatalogExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Only translate business exceptions that carry their own HTTP status; anything else
        // is left to the next handler (or the default 500) so we never mask a real failure.
        if (exception is not TvShowsCatalogException domainException)
        {
            return false;
        }

        var status = (int)domainException.Status;

        if (status >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled TV shows catalog exception");
        }
        else
        {
            logger.LogWarning("TV shows catalog exception: {Message}", exception.Message);
        }

        // Set the real HTTP status code before writing the body: ProblemDetails.Status alone
        // is just a JSON field and does not change the actual response status.
        httpContext.Response.StatusCode = status;

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = ReasonPhrases.GetReasonPhrase(status),
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        // Never leak internals in production: the stack trace is exposed in development only.
        if (environment.IsDevelopment())
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }
}
