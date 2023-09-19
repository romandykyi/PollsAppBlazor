using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollsAppBlazor.Server.Services;

namespace PollsAppBlazor.Server.Controllers
{
	[ApiController]
	[Route("api/options")]
	// doesn't work with Swagger: [AutoValidateAntiforgeryToken]
	public class OptionsController : ControllerBase
	{
		private readonly IOptionsService _optionsService;
		private readonly IVotesService _votesService;

		public OptionsController(IOptionsService optionsService, IVotesService votesService)
		{
			_optionsService = optionsService;
			_votesService = votesService;
		}

		/// <summary>
		/// Votes for selected option
		/// </summary>
		/// <response code="204">Success</response>
		/// <response code="401">Unauthorized user call</response>
		/// <response code="403">User lacks permission to vote</response>
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
			// Check whether has Poll assigned to it
			int? pollId = await _optionsService.GetPollIdAsync(optionId);
			if (pollId == null)
			{
				return NotFound();
			}
			// Make sure that user isn't voting twice
			string userId = User.GetSubjectId();
			if (await _votesService.GetVotedOptionAsync(pollId.Value, userId) != null)
			{
				return Forbid();
			}

			// Vote
			await _votesService.VoteAsync(pollId.Value, optionId, userId);

			return NoContent();
		}
	}
}
