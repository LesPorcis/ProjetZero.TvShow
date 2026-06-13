using Microsoft.AspNetCore.Mvc;
using TvShow.Api.Mappers;
using TvShow.Api.ViewModels;
using TvShow.Application.Ports.In;

namespace TvShow.Api.Controllers;

[ApiController]
[Route("api/tvshows")]
[Produces("application/json")]
public sealed class TvShowsController(IListTvShowsUseCase listTvShowsUseCase) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<TvShowViewModel>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TvShowViewModel>>> GetAll(CancellationToken cancellationToken)
    {
        var tvShows = await listTvShowsUseCase.ExecuteAsync(cancellationToken);

        return Ok(tvShows.ToViewModels());
    }
}
