using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.Server.Data;
using PollsAppBlazor.Server.Models;
using PollsAppBlazor.Server.Services;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Server.Controllers
{
	[Route("api/user")]
	public class UserController : ControllerBase
	{
		private readonly IPollsService _pollsService;
		private readonly ApplicationDbContext _dataContext;
		private readonly UserManager<ApplicationUser> _userManager;

		public UserController(IPollsService pollsService, ApplicationDbContext dataContext,
			UserManager<ApplicationUser> userManager)
		{
			_pollsService = pollsService;
			_dataContext = dataContext;
			_userManager = userManager;
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
			string userId = User.Identity.GetSubjectId();

			var user = await _userManager.Users
				.FirstAsync(u => u.Id == userId);

			var polls = _dataContext
				.Entry(user)
				.Collection(user => user.CreatedPolls!)
				.Query();

			return Ok(await _pollsService.FilterPollsAsync(filter, polls));
		}
	}
}
