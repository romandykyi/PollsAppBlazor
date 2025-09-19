using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using PollsAppBlazor.Application.Services.Communication.Interfaces;
using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Server.Policy;
using PollsAppBlazor.Shared.Auth;
using PollsAppBlazor.Shared.Users;
using System.Text;

namespace PollsAppBlazor.Server.Controllers;

[Route("api/auth")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    IUserStore<ApplicationUser> userStore,
    SignInManager<ApplicationUser> signInManager,
    IEmailService emailService)
    : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUserStore<ApplicationUser> _userStore = userStore;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IUserEmailStore<ApplicationUser> _emailStore = (IUserEmailStore<ApplicationUser>)userStore;
    private readonly IEmailService _emailService = emailService;

    private const string confirmationEmailBody = @"
<p>Hi {0},</p>

<p>Thank you for registering with <strong>Polls App</strong>! Before you can start creating and participating in polls, we need to verify your email address.</p>

<p>Please confirm your email by clicking the link below:</p>

<p><a href='{1}' style='background-color:#4CAF50; color:white; padding:10px 20px; text-decoration:none; border-radius:5px;'>Confirm My Email</a></p>

<p>If you did not register for Polls App Blazor, please ignore this email.</p>

<p>Welcome aboard,<br>
<strong>Roman Dykyi</strong></p>";

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
            if (user == null) return Unauthorized(InvalidLoginAttemptResponse.Default);

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
    public async Task<IActionResult> Register([FromBody] UserRegisterDto user, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var newUser = Activator.CreateInstance<ApplicationUser>();

            await _userStore.SetUserNameAsync(newUser, user.Username, CancellationToken.None);
            await _emailStore.SetEmailAsync(newUser, user.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (result.Succeeded)
            {
                string baseUri = $"{Request.Scheme}://{Request.Host}";
                string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                string urlToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));
                string confirmationLink = $"{baseUri}/confirm-email?userId={newUser.Id}&token={urlToken}";

                bool sendResult = await _emailService.SendAsync(
                    user.Email,
                    "Please Confirm Your Email for Polls App Blazor",
                    string.Format(confirmationEmailBody, user.Username, confirmationLink),
                    cancellationToken);
                if (!sendResult)
                {
                    ModelState.AddModelError(string.Empty, "Failed to send confirmation email.");
                    return BadRequest(ModelState);
                }
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
    /// Confirms user's email address.
    /// </summary>
    /// <response code="204">Success</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">Invalid email confirmation attempt</response>
    [HttpPost]
    [Route("confirm-email")]
    [EnableRateLimiting(RateLimitingPolicy.RegisterPolicy)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Unauthorized();

        string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        return result.Succeeded ? NoContent() : Unauthorized();
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
