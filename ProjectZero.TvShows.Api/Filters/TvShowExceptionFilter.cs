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
                    Detail = $"TV show with id {exception.TvShowId.Value} was not found."
                });
                context.ExceptionHandled = true;
                break;

            case RelatedEntityNotFoundException exception:
                context.Result = new BadRequestObjectResult(new ProblemDetails
                {
                    Title = "Related entity not found",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = exception.Message
                });
                context.ExceptionHandled = true;
                break;

            case InvalidTvShowUpdateException exception:
                context.Result = new BadRequestObjectResult(new ProblemDetails
                {
                    Title = "Invalid TV show update",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = exception.Message
                });
                context.ExceptionHandled = true;
                break;
        }
    }
}
