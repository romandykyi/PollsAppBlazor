using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Server.Policy;
using PollsAppBlazor.Shared.Auth;
using PollsAppBlazor.Shared.Users;

namespace PollsAppBlazor.Server.Controllers;

[Route("api/auth")]
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
    /// <response code="400">Validation failed</response>
    /// <response code="401">Invalid login attempt</response>
    /// <response code="403">Account is not confirmed</response>
    [HttpPost]
    [Route("login")]
    [EnableRateLimiting(RateLimitingPolicy.LogInPolicy)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> LogIn([FromBody] UserLoginDto login)
    {
        if (ModelState.IsValid)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(login.EmailOrUsername) ??
                await _signInManager.UserManager.FindByNameAsync(login.EmailOrUsername);
            if (user == null) return Unauthorized();

            if (!user.EmailConfirmed)
            {
                InvalidLoginAttemptResponse response = new()
                {
                    ErrorMessage = "You must have a confirmed email to log in.",
                    FailureReason = InvalidLoginAttemptResponse.Reason.EmailNotConfirmed
                };
                return Unauthorized(response);
            }

            var result = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return NoContent();
            }
            // Only disclose lockout error if password is correct to prevent user enumeration
            if (result.IsLockedOut && await _userManager.CheckPasswordAsync(user, login.Password))
            {
                InvalidLoginAttemptResponse response = new()
                {
                    ErrorMessage = "User account locked out, please try again.",
                    FailureReason = InvalidLoginAttemptResponse.Reason.LockedOut
                };
                return Unauthorized(response);
            }
        }

        // If we got this far, something failed
        return Unauthorized(InvalidLoginAttemptResponse.Default);
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
    [EnableRateLimiting(RateLimitingPolicy.RegisterPolicy)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
