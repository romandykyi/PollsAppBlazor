using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using PollsAppBlazor.Server.Extensions;
using PollsAppBlazor.Server.Services;

namespace PollsAppBlazor.Server.Policy
{
	public class OptionEditAuthorizationHandler : EditAuthorizationHandler<OptionEditAuthorizationRequirement>
	{
		private readonly IPollsService _pollsService;
		private readonly IOptionsService _optionsService;
		private readonly ILogger<OptionEditAuthorizationHandler> _logger;

		public OptionEditAuthorizationHandler(IPollsService pollsService,
			IOptionsService optionsService,
			ILogger<OptionEditAuthorizationHandler> logger)
		{
			_pollsService = pollsService;
			_optionsService = optionsService;
			_logger = logger;
		}

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
			OptionEditAuthorizationRequirement requirement)
		{
			if (!MustBeCreator(context.User))
			{
				// User is permitted to edit any Option
				context.Succeed(requirement);
				return;
			}

			int optionId = GetIntIdFromRoute(context, "optionId");

			// Try to get an ID of the Poll
			int? pollId = await _optionsService.GetPollIdAsync(optionId);
			if (pollId == null)
			{
				// Option doesn't exist, so we fail
				context.Fail();
				return;
			}
			// Check whether user is a creator of the Poll
			string? creatorId = await _pollsService.GetCreatorIdAsync(pollId.Value);
			if (creatorId == null)
			{
				_logger.LogError("Poll {PollId}, related to Option {OptionId}, doesn't exist",
					pollId, optionId);
				context.Fail();
			}
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
