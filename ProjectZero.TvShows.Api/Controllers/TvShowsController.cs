using Microsoft.AspNetCore.Mvc;
using ProjectZero.TvShows.Api.Mappers;
using ProjectZero.TvShows.Api.ViewModels;
using ProjectZero.TvShows.Application.Ports.In;

namespace ProjectZero.TvShows.Api.Controllers;

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
