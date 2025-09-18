using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Server.Policy;
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
    /// <response code="400">Invalid login attempt</response>
    [HttpPost]
    [Route("login")]
    [EnableRateLimiting(RateLimitingPolicy.LogInPolicy)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LogIn([FromBody] UserLoginDto login)
    {
        if (ModelState.IsValid)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(login.EmailOrUsername) ??
                await _signInManager.UserManager.FindByNameAsync(login.EmailOrUsername);
            if (user == null) return Unauthorized();

            var result = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return NoContent();
            }
            if (result.IsLockedOut)
            {
                // Only add lockout error if password is correct to prevent user enumeration
                if (await _userManager.CheckPasswordAsync(user, login.Password))
                {
                    ModelState.AddModelError(string.Empty, "User account locked out, please try again.");
                    return BadRequest(ModelState);
                }
                return Unauthorized();
            }
        }

        // If we got this far, something failed
        return Unauthorized();
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
