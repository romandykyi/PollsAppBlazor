using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PollsAppBlazor.Server.Policy;
using PollsAppBlazor.Server.Services;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Server.Controllers
{
	[ApiController]
	[Route("api/polls")]
	// doesn't work with Swagger: [AutoValidateAntiforgeryToken]
	public class PollsController : ControllerBase
	{
		private readonly IPollsService _pollsService;
		private readonly IOptionsService _optionsService;

		public PollsController(IPollsService pollsService, IOptionsService optionsService)
		{
			_pollsService = pollsService;
			_optionsService = optionsService;
		}

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
			// PollsService only checks whether user is an owner,
			// but administrators and moderators can edit too.
			// This probably needs to be refactored
			if (!result.CurrentUserCanEdit && Policies.UserCanEditAnything(User))
			{
				result.CurrentUserCanEdit = true;
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
		public async Task<IActionResult> Get([FromQuery] PollsPageFilter filter)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			return Ok(await _pollsService.GetPollsAsync(filter));
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
				true => NoContent(),
				false => Forbid(),
				_ => NotFound()
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
	}
}
