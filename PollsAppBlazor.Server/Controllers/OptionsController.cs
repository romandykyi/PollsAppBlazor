using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PollsAppBlazor.Application.Services.Interfaces;
using PollsAppBlazor.Application.Services.Results;
using PollsAppBlazor.Server.Policy;

namespace PollsAppBlazor.Server.Controllers;

[ApiController]
[Route("api/options")]
public class OptionsController(IVoteService votesService) : ControllerBase
{
    private readonly IVoteService _votesService = votesService;

    private ObjectResult ForbidVote(string detail)
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
    [EnableRateLimiting(RateLimitingPolicy.VotePolicy)]
    [Route("{optionId}/vote")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Vote([FromRoute] int optionId, CancellationToken cancellationToken)
    {
        return await _votesService.VoteAsync(optionId, User.GetSubjectId(), cancellationToken) switch
        {
            VoteServiceResult.Success => NoContent(),
            VoteServiceResult.PollExpired => ForbidVote("The poll is not available for voting."),
            VoteServiceResult.PollNotFound => NotFound(),
            VoteServiceResult.AlreadyVoted => ForbidVote("Cannot vote twice."),
            _ => throw new InvalidOperationException("Unsupported VoteServiceResult value.")
        };
    }
}
