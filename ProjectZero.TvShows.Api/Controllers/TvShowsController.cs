using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectZero.TvShows.Api.Mappers;
using ProjectZero.TvShows.Api.ViewModels;
using ProjectZero.TvShows.Application.Ports.In;

namespace ProjectZero.TvShows.Api.Controllers;

[ApiController]
[Route("api/tvshows")]
[Produces("application/json")]
public sealed class TvShowsController(ITvShowsCatalog tvShowsCatalog) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<TvShowViewModel>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TvShowViewModel>>> ListAsync(CancellationToken cancellationToken)
    {
        var tvShows = await tvShowsCatalog.ListAsync(cancellationToken);

        return Ok(tvShows.ToViewModels());
    }
}
