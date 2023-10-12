using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using PollsAppBlazor.Server.Services;

namespace PollsAppBlazor.Server.Policy
{
	public sealed class PollEditAuthorizationHandler : PollEditAuthorizationHandler<PollEditAuthorizationRequirement>
	{
		public PollEditAuthorizationHandler(PollsService pollsService) : base(pollsService) { }
	}

	public abstract class PollEditAuthorizationHandler<TRequirement> : EditAuthorizationHandler<TRequirement>
		where TRequirement : IAuthorizationRequirement
	{
		private readonly PollsService _pollsService;

		public PollEditAuthorizationHandler(PollsService pollsService)
		{
			_pollsService = pollsService;
		}

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
			TRequirement requirement)
		{
			if (BasicCheck(context, requirement)) return;

			// Check whether user is a creator of the Poll
			int pollId = GetIntIdFromRoute(context, "pollId");
			string? creatorId = await _pollsService.GetCreatorIdAsync(pollId);
			if (creatorId == context.User.GetSubjectId())
			{
				context.Succeed(requirement);
			}
			else
			{
				context.Fail();
			}
		}
	}
}
