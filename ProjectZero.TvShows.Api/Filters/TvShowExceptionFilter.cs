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
            case NotFoundException { TvShowId: not null } exception:
                context.Result = new NotFoundObjectResult(new ProblemDetails
                {
                    Title = "TV show not found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = exception.Message
                });
                context.ExceptionHandled = true;
                break;

            case NotFoundException exception:
                context.Result = new BadRequestObjectResult(new ProblemDetails
                {
                    Title = "Related entity not found",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = exception.Message
                });
                context.ExceptionHandled = true;
                break;

            case InvalidTvShowUpdateException exception:
                if (exception.Errors.Count > 0)
                {
                    var problem = new ValidationProblemDetails
                    {
                        Title = "Invalid TV show update",
                        Status = StatusCodes.Status400BadRequest
                    };

                    foreach (var (field, messages) in exception.Errors)
                    {
                        problem.Errors[field] = messages;
                    }

                    context.Result = new BadRequestObjectResult(problem);
                }
                else
                {
                    context.Result = new BadRequestObjectResult(new ProblemDetails
                    {
                        Title = "Invalid TV show update",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = exception.Message
                    });
                }

                context.ExceptionHandled = true;
                break;
        }
    }
}
