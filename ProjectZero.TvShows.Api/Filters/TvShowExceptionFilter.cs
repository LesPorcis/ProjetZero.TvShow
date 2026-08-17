using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectZero.TvShows.Application.Ports.In;

namespace ProjectZero.TvShows.Api.Filters;

internal sealed class TvShowsExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is TvShowNotFoundException exception)
        {
            context.Result = new NotFoundObjectResult(new ProblemDetails
            {
                Title = "TV show not found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"TV show with id {exception.TvShowId.Value} was not found."
            });

            context.ExceptionHandled = true;
        }
    }
}
