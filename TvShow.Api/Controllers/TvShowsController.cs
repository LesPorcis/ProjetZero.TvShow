using Microsoft.AspNetCore.Mvc;
using TvShow.Application.Models;
using TvShow.Application.Ports.In;

namespace TvShow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TvShowsController(IListTvShowsUseCase listTvShowsUseCase ) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TvShowResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var tvShows = await listTvShowsUseCase.ExecuteAsync();

        return Ok(tvShows);
    }
}
