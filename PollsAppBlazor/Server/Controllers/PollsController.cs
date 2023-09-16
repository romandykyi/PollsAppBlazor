using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollsAppBlazor.Server.Extensions;
using PollsAppBlazor.Server.Policy;
using PollsAppBlazor.Server.Services;
using PollsAppBlazor.Shared;

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
		[Route("{pollId}")]
		[ProducesResponseType(typeof(PollViewDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Get([FromRoute] int pollId)
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
		/// Gets latest Polls list
		/// </summary>
		/// <response code="200">Returns Polls list</response>
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<PollPreviewDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> Get()
		{
			return Ok(await _pollsService.GetNewestPollsAsync(10));
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
		/// The response includes the "errors" key with an array of errors, 
		/// where each element's key represents an invalid property, 
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

			return CreatedAtAction(nameof(Get), routeValues, result);
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
		/// The response includes the "errors" key with an array of errors, 
		/// where each element's key represents an invalid property, 
		/// and the corresponding value describes the reason for the error
		/// </response>
		/// <response code="401">Unauthorized user call</response>
		/// <response code="403">User lacks permission to edit this Poll</response>
		/// <response code="404">The Poll does not exist</response>
		[HttpPatch()]
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
			if (await _pollsService.EditPollAsync(poll, pollId))
			{
				return NoContent();
			}
			return NotFound();
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
