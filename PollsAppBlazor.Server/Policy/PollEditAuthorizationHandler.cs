using Microsoft.AspNetCore.Authorization;
using PollsAppBlazor.Application.Services.Interfaces;
using PollsAppBlazor.Server.Extensions.Utils;

namespace PollsAppBlazor.Server.Policy;

public sealed class PollEditAuthorizationHandler(IPollService pollsService) :
    PollEditAuthorizationHandler<PollEditAuthorizationRequirement>(pollsService)
{
}

public abstract class PollEditAuthorizationHandler<TRequirement>(IPollService pollsService) : EditAuthorizationHandler<TRequirement>
    where TRequirement : IAuthorizationRequirement
{
    private readonly IPollService _pollsService = pollsService;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        TRequirement requirement)
    {
        if (BasicCheck(context, requirement)) return;

        // Check whether user is a creator of the Poll
        int pollId = GetIntIdFromRoute(context, "pollId");
        string? creatorId = await _pollsService.GetCreatorIdAsync(pollId, CancellationToken.None);
        if (creatorId == context.User.GetUserId())
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
