using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectZero.TvShows.Api.Mappers;
using ProjectZero.TvShows.Api.Requests;
using ProjectZero.TvShows.Api.ViewModels;
using ProjectZero.TvShows.Application.Ports.In;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Api.Controllers;

[ApiController]
[Route("api/tv-shows")]
[Produces("application/json")]
public sealed class TvShowsController(
    ITvShowsCatalog tvShowsCatalog,
    ITvShowsCreation tvShowsCreation) : ControllerBase
{
    private const string GetByIdRouteName = "GetTvShowById";

    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<TvShowViewModel>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TvShowViewModel>>> ListAsync(CancellationToken cancellationToken)
    {
        var tvShows = await tvShowsCatalog.ListAsync(cancellationToken);

        return Ok(tvShows.ToViewModels());
    }

    [HttpGet("{tvShowId:int:min(1)}", Name = GetByIdRouteName)]
    [ProducesResponseType<TvShowViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TvShowViewModel>> GetByIdAsync(int tvShowId, CancellationToken cancellationToken)
    {
        var tvShow = await tvShowsCatalog.GetByIdAsync(new TvShowId(tvShowId), cancellationToken);

        return Ok(tvShow.ToViewModel());
    }

    [HttpPost]
    [ProducesResponseType<TvShowViewModel>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TvShowViewModel>> CreateAsync(
        CreateTvShowRequest request,
        CancellationToken cancellationToken)
    {
        var tvShow = await tvShowsCreation.CreateAsync(request.ToCommand(), cancellationToken);

        return CreatedAtRoute(
            GetByIdRouteName,
            new { tvShowId = tvShow.Id.Value },
            tvShow.ToViewModel());
    }
}
