using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PollsAppBlazor.Server.Models;
using PollsAppBlazor.Shared.Users;

namespace PollsAppBlazor.Server.Controllers
{
	[Route("api/auth")]
	// doesn't work with Swagger: [AutoValidateAntiforgeryToken]
	public class AuthController : ControllerBase
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserStore<ApplicationUser> _userStore;
		private readonly IUserEmailStore<ApplicationUser> _emailStore;

		public AuthController(
			UserManager<ApplicationUser> userManager,
			IUserStore<ApplicationUser> userStore,
			SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_userStore = userStore;
			_emailStore = (IUserEmailStore<ApplicationUser>)_userStore;
			_signInManager = signInManager;
		}

		/// <summary>
		/// Logs in a user
		/// </summary>
		/// <response code="204">Success</response>
		/// <response code="400">Invalid login attempt</response>
		[HttpPost]
		[Route("login")]
		[AllowAnonymous]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> LogIn([FromBody] UserLoginDto login)
		{
			if (ModelState.IsValid)
			{
				var user = await _signInManager.UserManager.FindByEmailAsync(login.EmailOrUsername);
				string username = (user != null ? user.UserName : login.EmailOrUsername)!;

				// This doesn't count login failures towards account lockout and two factor authorization
				var result = await _signInManager.PasswordSignInAsync(username, login.Password, login.RememberMe, lockoutOnFailure: false);
				if (result.Succeeded)
				{
					return NoContent();
				}
				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
			}

			// If we got this far, something failed
			return BadRequest(ModelState);
		}

		/// <summary>
		/// Registers a user
		/// </summary>
		/// <response code="204">Success</response>
		/// <response code="400">
		/// Register failed, body contains arrays of errors
		/// </response>
		[HttpPost]
		[Route("register")]
		[AllowAnonymous]
		public async Task<IActionResult> Register([FromBody] UserRegisterDto user)
		{
			if (ModelState.IsValid)
			{
				var newUser = Activator.CreateInstance<ApplicationUser>();

				await _userStore.SetUserNameAsync(newUser, user.Username, CancellationToken.None);
				await _emailStore.SetEmailAsync(newUser, user.Email, CancellationToken.None);
				var result = await _userManager.CreateAsync(newUser, user.Password);

				if (result.Succeeded)
				{
					await _signInManager.SignInAsync(newUser, isPersistent: false);
					return NoContent();
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			// If we got this far, something failed
			return BadRequest(ModelState);
		}

		/// <summary>
		/// Logs out a user
		/// </summary>
		/// <response code="204">Success</response>
		/// <response code="401">Unauthorized user call</response>
		[HttpPost]
		[Route("logout")]
		[Authorize]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();

			return NoContent();
		}
	}
}
