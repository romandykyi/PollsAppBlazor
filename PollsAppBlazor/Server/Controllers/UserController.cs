using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PollsAppBlazor.Server.Services;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Server.Controllers
{
	[Route("api/user")]
	public class UserController : ControllerBase
	{
		private readonly IPollsService _pollsService;

		public UserController(IPollsService pollsService)
		{
			_pollsService = pollsService;
		}

		/// <summary>
		/// Gets polls created by the current user
		/// </summary>
		/// <remarks>
		/// Creator property is ignored.
		/// </remarks>
		/// <response code="200">Returns Polls page that match the filter and are created by the current user</response>
		/// <response code="400">
		/// Malformed or invalid input. 
		/// <br />
		/// The response includes the "errors" key with properties that contain arrays of 
		/// validation errors
		/// and the corresponding value describes the reason for the error
		/// </response>
		/// <response code="401">Unauthorized user call</response>
		[HttpGet]
		[AllowAnonymous]
		[Route("polls")]
		[ProducesResponseType(typeof(PollsPage), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(BadRequest), StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetCreatedPolls([FromQuery] PollsPageFilter filter)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(await _pollsService.GetUserPollsAsync(filter, User.Identity.GetSubjectId()));
		}
	}
}
