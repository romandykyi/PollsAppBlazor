using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PollsAppBlazor.Application.Services.Interfaces;
using PollsAppBlazor.Application.Services.Results;
using PollsAppBlazor.Server.Extensions.Utils;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Server.Controllers;

[Authorize]
[Route("api/user")]
public class UserController(
    IUserService userService,
    IFavoriteService favoritesService
    ) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IFavoriteService _favoritesService = favoritesService;

    /// <summary>
    /// Gets polls created by the current user
    /// </summary>
    /// <remarks>
    /// Creator query property is ignored.
    /// </remarks>
    /// <response code="200">Returns Polls page that match the filter and are created by the current user</response>
    /// <response code="400">
    /// Malformed or invalid input. 
    /// <br />
    /// The response includes the "errors" key with properties that contain arrays of 
    /// validation errors
    /// and the corresponding value describes the reason for the error
    /// </response>
    /// <response code="401">Unauthorized user call</response>
    [HttpGet]
    [Route("polls")]
    [ProducesResponseType(typeof(PollsPage), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCreatedPolls(
        [FromQuery] PollsPagePaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        string userId = User.GetUserId();

        return Ok(await _userService.GetPollsAsync(parameters, userId, cancellationToken));
    }

    /// <summary>
    /// Gets favorite polls of the current user(with filter)
    /// </summary>
    /// <remarks>
    /// Creator query property is ignored.
    /// </remarks>
    /// <response code="200">Returns Polls page that match the filter and are favorite</response>
    /// <response code="400">
    /// Malformed or invalid input. 
    /// <br />
    /// The response includes the "errors" key with properties that contain arrays of 
    /// validation errors
    /// and the corresponding value describes the reason for the error
    /// </response>
    /// <response code="401">Unauthorized user call</response>
    [HttpGet]
    [Route("favorites")]
    [ProducesResponseType(typeof(PollsPage), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFavoritePolls(
        [FromQuery] PollsPagePaginationParameters parameters,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        string userId = User.GetUserId();

        return Ok(await _userService.GetFavoritePollsAsync(parameters, userId, cancellationToken));
    }

    /// <summary>
    /// Adds (or removes) the poll to favorites for the current user
    /// </summary>
    /// <response code="204">Poll's favorite status was changed or already or was already in the requested state.</response>
    /// <response code="401">Unauthorized user call</response>
    /// <response code="404">Poll was not found</response>
    /// <response code="410">Poll was deleted</response>
    [HttpPut]
    [Route("favorites/{pollId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<IActionResult> SetFavoritesState(
        [FromRoute] int pollId,
        [FromBody] SetPollFavoriteStateDto dto,
        CancellationToken cancellationToken
        )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string userId = User.GetUserId();
        var result = await _favoritesService
            .SetPollFavoriteState(pollId, userId, dto.IsInFavorites, cancellationToken);

        return result switch
        {
            SetFavoriteStateResult.Success => NoContent(),
            SetFavoriteStateResult.NotFound => NotFound(),
            SetFavoriteStateResult.Deleted => StatusCode(StatusCodes.Status410Gone),
            _ => throw new InvalidOperationException($"Invalid {nameof(SetFavoriteStateResult)} value: {result}.")
        };
    }
}
