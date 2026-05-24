using Microsoft.AspNetCore.Mvc;
using TvShow.Application.Models;
using TvShow.Application.Services;

namespace TvShow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TvShowsController(TvShowManager tvShowManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TvShowResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var tvShows = await tvShowManager.GetAllAsync(cancellationToken);

        return Ok(tvShows);
    }
}
