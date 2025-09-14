using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PollsAppBlazor.Application.Services.Implementations;
using PollsAppBlazor.Application.Services.Interfaces;
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
    public async Task<IActionResult> GetCreatedPolls([FromQuery] PollsPagePaginationParameters parameters)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        string userId = User.GetSubjectId();

        return Ok(await _userService.GetPollsAsync(parameters, userId));
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
    public async Task<IActionResult> GetFavoritePolls([FromQuery] PollsPagePaginationParameters parameters)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        string userId = User.Identity.GetSubjectId();

        return Ok(await _userService.GetFavoritePollsAsync(parameters, userId));
    }

    /// <summary>
    /// Adds a poll to favorites for the current user
    /// </summary>
    /// <response code="204">Poll was added to favorites</response>
    /// <response code="401">Unauthorized user call</response>
    /// <response code="404">Poll was not found</response>
    [HttpPut]
    [Route("favorites/{pollId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddToFavorites([FromRoute] int pollId)
    {
        if (await _favoritesService.AddToFavoritesAsync(pollId, User.GetSubjectId()))
        {
            return NoContent();
        }
        return NotFound();
    }

    /// <summary>
    /// Removes a poll from the favorites for the current user
    /// </summary>
    /// <response code="204">Poll was removed from favorites</response>
    /// <response code="401">Unauthorized user call</response>
    /// <response code="404">Poll wasn't in favorites</response>
    [HttpDelete]
    [Route("favorites/{pollId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove([FromRoute] int pollId)
    {
        if (await _favoritesService.RemoveFromFavoritesAsync(pollId, User.GetSubjectId()))
        {
            return NoContent();
        }
        return NotFound();
    }
}
