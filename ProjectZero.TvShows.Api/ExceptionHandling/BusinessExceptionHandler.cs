using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectZero.TvShows.Application.Exceptions;

namespace ProjectZero.TvShows.Api.ExceptionHandling;

internal sealed class BusinessExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment,
    ILogger<BusinessExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Only business exceptions are translated here; anything else is left to the default
        // 500 so we never mask a real failure.
        if (exception is not BusinessException businessException)
        {
            return false;
        }

        // The core exposes a transport-agnostic error kind; mapping it to an HTTP status is the
        // driving adapter's responsibility, so the switch lives here and not in the core.
        var status = businessException.Kind switch
        {
            ErrorKind.NotFound => StatusCodes.Status404NotFound,
            ErrorKind.Validation => StatusCodes.Status400BadRequest,
            ErrorKind.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        if (status >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled business exception");
        }
        else
        {
            logger.LogWarning("Business exception: {Message}", exception.Message);
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
