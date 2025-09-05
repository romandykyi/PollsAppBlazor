using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollsAppBlazor.Application.Services.Implementations;

namespace PollsAppBlazor.Server.Controllers;

[ApiController]
[Route("api/options")]
// doesn't work with Swagger: [AutoValidateAntiforgeryToken]
public class OptionsController : ControllerBase
{
    private readonly PollsService _pollsService;
    private readonly OptionsService _optionsService;
    private readonly VotesService _votesService;

    public OptionsController(PollsService pollsService,
        OptionsService optionsService, VotesService votesService)
    {
        _pollsService = pollsService;
        _optionsService = optionsService;
        _votesService = votesService;
    }

    private IActionResult ForbidVote(string detail)
    {
        return Problem(
                type: "/docs/errors/forbidden",
                title: "Vote is forbidden.",
                detail: detail,
                statusCode: StatusCodes.Status403Forbidden,
                instance: HttpContext.Request.Path
            );
    }

    /// <summary>
    /// Votes for selected option
    /// </summary>
    /// <response code="204">Success</response>
    /// <response code="401">Unauthorized user call</response>
    /// <response code="403">User lacks permission to vote or Poll is not active</response>
    /// <response code="404">The Option does not exist</response>
    [HttpPost]
    [Authorize]
    [Route("{optionId}/vote")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Vote([FromRoute] int optionId)
    {
        // Check whether option has Poll assigned to it
        int? pollId = await _optionsService.GetPollIdAsync(optionId);
        if (pollId == null)
        {
            return NotFound();
        }
        // Make sure that poll is active
        if (await _pollsService.IsPollActiveAsync(pollId.Value) == true)
        {
            return ForbidVote("Poll is not active.");
        }
        // Make sure that user isn't voting twice
        string userId = User.GetSubjectId();
        if (await _votesService.GetVotedOptionAsync(pollId.Value, userId) != null)
        {
            return ForbidVote("User cannot vote twice on the same Poll.");
        }

        // Vote
        await _votesService.VoteAsync(pollId.Value, optionId, userId);

        return NoContent();
    }
}
