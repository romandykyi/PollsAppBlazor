using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PollsAppBlazor.Application.Services.Implementations;
using PollsAppBlazor.Application.Services.Results;
using PollsAppBlazor.Server.Policy;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Server.Controllers;

[ApiController]
[Route("api/polls")]
// doesn't work with Swagger: [AutoValidateAntiforgeryToken]
public class PollsController(PollsService pollsService) : ControllerBase
{
    private readonly PollsService _pollsService = pollsService;

    /// <summary>
    /// Gets a Poll by its ID
    /// </summary>
    /// <remarks>
    /// Options of the poll will contain votes if user is allowed to see them.
    /// </remarks>
    /// <response code="200">Returns requested Poll</response>
    /// <response code="404">The Poll does not exist</response>
    [HttpGet]
    [AllowAnonymous]
    [Route("{pollId}")]
    [ProducesResponseType(typeof(PollViewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int pollId)
    {
        string? userId = User.IsAuthenticated() ? User.GetSubjectId() : null;
        PollViewDto? result = await _pollsService.GetByIdAsync(pollId, userId);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets Polls page with a filter
    /// </summary>
    /// <response code="200">Returns Polls page that match the filter</response>
    /// <response code="400">
    /// Malformed or invalid input. 
    /// <br />
    /// The response includes the "errors" key with properties that contain arrays of 
    /// validation errors
    /// and the corresponding value describes the reason for the error
    /// </response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PollsPage), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromQuery] PollsPagePaginationParameters filter)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        return Ok(await _pollsService.GetPollsAsync(filter));
    }

    /// <summary>
    /// Gets Poll options with votes count.
    /// </summary>
    /// <returns>The request options with votes count.</returns>
    /// <response code="200">Returns requested options</response>
    /// <response code="401">Unauthorized user call</response>
    /// <response code="403">User lacks permission to see votes for this Poll</response>
    /// <response code="404">The Poll does not exist</response>
    [HttpGet("{pollId}/options")]
    [Authorize]
    [ProducesResponseType(typeof(PollViewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOptions([FromRoute] int pollId)
    {
        var result = await _pollsService.GetOptionsWithVotesAsync(pollId);

        return result.Status switch
        {
            GetOptionsWithVotesStatus.Success => Ok(result.Options),
            GetOptionsWithVotesStatus.PollNotFound => NotFound(),
            GetOptionsWithVotesStatus.NotVisible => Forbid(),
            _ => throw new InvalidOperationException("Unknown GetOptionsWithVotesStatus")
        };
    }

    /// <summary>
    /// Creates a Poll
    /// </summary>
    /// <response code="201">
    /// Poll was successfully created. The response body contains created Poll
    /// </response>
    /// <response code="400">
    /// Malformed or invalid input. 
    /// <br />
    /// The response includes the "errors" key with properties that contain arrays of 
    /// validation errors
    /// and the corresponding value describes the reason for the error
    /// </response>
    /// <response code="401">Unauthorized user call</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(PollViewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] PollCreationDto poll)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string userId = User.GetSubjectId();
        PollViewDto result = await _pollsService.CreatePollAsync(poll, userId);
        var routeValues = new { pollId = result.Id };

        return CreatedAtAction(nameof(GetById), routeValues, result);
    }

    /// <summary>
    /// Edits a Poll
    /// </summary>
    /// <remarks>
    /// All options in the response body are optional.
    /// </remarks>
    /// <response code="204">
    /// The Poll was successfully edited or the request body contains no properties
    /// </response>
    /// <response code="400">
    /// Malformed or invalid input. 
    /// <br />
    /// The response includes the "errors" key with properties that contain arrays of 
    /// validation errors
    /// </response>
    /// <response code="401">Unauthorized user call</response>
    /// <response code="403">
    /// User lacks permission to edit this Poll or Poll is not active
    /// </response>
    /// <response code="404">The Poll does not exist</response>
    [HttpPatch]
    [Authorize(Policy = Policies.CanEditPoll)]
    [Route("{pollId}")]
    [ProducesResponseType(typeof(PollViewDto), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Edit([FromBody] PollEditDto poll, [FromRoute] int pollId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return await _pollsService.EditPollAsync(poll, pollId) switch
        {
            EditPollResult.Success => NoContent(),
            EditPollResult.Expired => Forbid(),
            EditPollResult.NotFound => NotFound(),
            _ => throw new InvalidOperationException("Unknown EditPollResult")
        };
    }

    /// <summary>
    /// Gets an editing representation for the Poll
    /// </summary>
    /// <response code="200">
    /// Returns requested editing representation of the Poll
    /// </response>
    /// <response code="401">Unauthorized user call</response>
    /// <response code="403">User lacks permission to edit this Poll</response>
    /// <response code="404">The Poll does not exist</response>
    [HttpGet]
    [Authorize(Policy = Policies.CanEditPoll)]
    [Route("{pollId}/edit")]
    [ProducesResponseType(typeof(PollViewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEdit([FromRoute] int pollId)
    {
        PollCreationDto? result = await _pollsService.GetForEditById(pollId);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Deletes a Poll
    /// </summary>
    /// <response code="204">The Poll was successfully deleted</response>
    /// <response code="401">Unauthorized user call</response>
    /// <response code="403">User lacks permission to edit this Poll</response>
    /// <response code="404">The Poll does not exist</response>
    [HttpDelete]
    [Authorize(Policy = Policies.CanEditPoll)]
    [Route("{pollId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int pollId)
    {
        if (await _pollsService.DeletePollAsync(pollId))
        {
            return NoContent();
        }
        return NotFound();
    }

    /// <summary>
    /// Makes a Poll expired
    /// </summary>
    /// <response code="204">The Poll was successfully made expired</response>
    /// <response code="401">Unauthorized user call</response>
    /// <response code="403">User lacks permission to edit this Poll</response>
    /// <response code="404">The Poll does not exist</response>
    /// <response code="422">The Poll is already expired</response>
    [HttpPatch]
    [Authorize(Policy = Policies.CanEditPoll)]
    [Route("{pollId}/expire")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Expire([FromRoute] int pollId)
    {
        return await _pollsService.ExpirePollAsync(pollId) switch
        {
            ExpirePollResult.Success => NoContent(),
            ExpirePollResult.CannotExpire => UnprocessableEntity(),
            ExpirePollResult.NotFound => NotFound(),
            _ => throw new InvalidOperationException("Unknown ExpirePollResult")
        };
    }
}
