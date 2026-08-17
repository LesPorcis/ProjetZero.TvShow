using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectZero.TvShows.Api.Mappers;
using ProjectZero.TvShows.Api.ViewModels;
using ProjectZero.TvShows.Application.Ports.In;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Api.Controllers;

[ApiController]
[Route("api/tv-shows")]
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

    [HttpGet("{tvShowId:int:min(1)}")]
    [ProducesResponseType<TvShowViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TvShowViewModel>> GetByIdAsync(int tvShowId, CancellationToken cancellationToken)
    {
        var tvShow = await tvShowsCatalog.GetByIdAsync(new TvShowId(tvShowId), cancellationToken);

        return Ok(tvShow.ToViewModel());
    }
}
