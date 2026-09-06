using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectZero.TvShows.Application.Exceptions;

namespace ProjectZero.TvShows.Api.Filters;

internal sealed class TvShowsExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case TvShowNotFoundException exception:
                context.Result = new NotFoundObjectResult(new ProblemDetails
                {
                    Title = "TV show not found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = exception.Message
                });
                break;

            case InvalidTvShowException exception:
                context.Result = new BadRequestObjectResult(new ValidationProblemDetails(
                    exception.Errors.ToDictionary(error => error.Key, error => error.Value))
                {
                    Title = "Invalid TV show",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = exception.Message
                });
                break;

            default:
                return;
        }

        context.ExceptionHandled = true;
    }
}
