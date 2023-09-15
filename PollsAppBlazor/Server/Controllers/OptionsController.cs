using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollsAppBlazor.Server.Policy;
using PollsAppBlazor.Server.Services;
using PollsAppBlazor.Shared;

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
		/// Edits an Option
		/// </summary>
		/// <response code="204">
		/// The Option was successfully edited
		/// </response>
		/// <response code="400">
		/// Malformed or invalid input. 
		/// <br />
		/// The response includes the "errors" key with an array of errors, 
		/// where each element's key represents an invalid property, 
		/// and the corresponding value describes the reason for the error
		/// </response>
		/// <response code="401">Unauthorized user call</response>
		/// <response code="403">User lacks permission to edit this Option</response>
		/// <response code="404">The Option does not exist</response>
		[HttpPut]
		[Authorize(Policy = Policies.CanEditOption)]
		[Route("{optionId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Edit([FromBody] OptionEditDto option, [FromRoute] int optionId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (await _optionsService.EditOptionAsync(option, optionId))
			{
				return NoContent();
			}
			return NotFound();
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
