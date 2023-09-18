using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using PollsAppBlazor.Server.Services;

namespace PollsAppBlazor.Server.Policy
{
	public sealed class PollEditAuthorizationHandler : PollEditAuthorizationHandler<PollEditAuthorizationRequirement>
	{
		public PollEditAuthorizationHandler(IPollsService pollsService) : base(pollsService) { }
	}

	public abstract class PollEditAuthorizationHandler<TRequirement> : EditAuthorizationHandler<TRequirement>
		where TRequirement : IAuthorizationRequirement
	{
		private readonly IPollsService _pollsService;

		public PollEditAuthorizationHandler(IPollsService pollsService)
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
