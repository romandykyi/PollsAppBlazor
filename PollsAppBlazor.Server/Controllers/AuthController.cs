using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.RateLimiting;
using PollsAppBlazor.Application.Auth;
using PollsAppBlazor.Server.Policy;
using PollsAppBlazor.Shared.Auth;
using PollsAppBlazor.Shared.Users;

namespace PollsAppBlazor.Server.Controllers;

[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

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
    [ProducesResponseType(typeof(InvalidLoginAttemptResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> LogIn([FromBody] UserLoginDto login, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LogInAsync(login, cancellationToken);

        if (result.Succeeded) return NoContent();

        InvalidLoginAttemptResponse response = result.FailureReason switch
        {
            LoginFailureReason.EmailNotConfirmed => new()
            {
                ErrorMessage = result.ErrorMessage ?? "Invalid login attempt.",
                FailureReason = InvalidLoginAttemptResponse.Reason.EmailNotConfirmed
            },
            LoginFailureReason.LockedOut => new()
            {
                ErrorMessage = result.ErrorMessage ?? "Invalid login attempt.",
                FailureReason = InvalidLoginAttemptResponse.Reason.LockedOut
            },
            _ => InvalidLoginAttemptResponse.Default
        };

        return Unauthorized(response);
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
    public async Task<IActionResult> Register([FromBody] UserRegisterDto user, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(user, cancellationToken);

        if (result.Status == RegisterStatus.Succeeded)
        {
            return NoContent();
        }

        if (result.Errors != null)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }
        else if (!result.EmailSent)
        {
            ModelState.AddModelError(string.Empty, "Failed to send confirmation email.");
        }

        return BadRequest(ModelState);
    }

    /// <summary>
    /// Confirms user's email address
    /// </summary>
    /// <response code="204">Success</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">Invalid email confirmation attempt</response>
    /// <response code="409">Email is already confirmed</response>
    [HttpPost]
    [Route("confirm-email")]
    [EnableRateLimiting(RateLimitingPolicy.RegisterPolicy)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return Conflict(new { Message = "You are already logged in" });
        }

        var result = await _authService.ConfirmEmailAsync(userId, token, cancellationToken);
        if (result.Status == ConfirmEmailStatus.InvalidToken)
        {
            ModelState.AddModelError("token", result.ErrorMessage ?? "Invalid token");
            return BadRequest(ModelState);
        }

        return result.Status switch
        {
            ConfirmEmailStatus.Succeeded => NoContent(),
            ConfirmEmailStatus.AlreadyConfirmed => Conflict(new { Message = result.ErrorMessage ?? "Already confirmed" }),
            _ => Unauthorized()
        };
    }

    /// <summary>
    /// Initiates password reset process by sending a reset link to the user's email
    /// </summary>
    /// <response code="204">Request accepted, but not guaranteed to be successful</response>
    /// <response code="400">Invalid request</response>
    [HttpPost]
    [Route("initiate-reset-password")]
    [EnableRateLimiting(RateLimitingPolicy.ResetPasswordPolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InitiatePasswordReset([FromBody] InitiateResetPasswordDto resetDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _ = await _authService.InitiatePasswordResetAsync(resetDto.Email, cancellationToken);

        // Prevent enumeration attacks
        return NoContent();
    }

    /// <summary>
    /// Resets user's password
    /// </summary>
    /// <response code="204">Success</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">Invalid reset password attempt</response>
    [HttpPost]
    [Route("reset-password")]
    [EnableRateLimiting(RateLimitingPolicy.LogInPolicy)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.ResetPasswordAsync(resetPasswordDto, cancellationToken);
        if (result.Status == ResetPasswordStatus.InvalidToken)
        {
            ModelState.AddModelError("token", result.ErrorMessage ?? "Invalid token");
            return BadRequest(ModelState);
        }
        if (result.Status == ResetPasswordStatus.PasswordRequirementsFailed)
        {
            foreach (var error in result.Errors!)
            {
                ModelState.AddModelError("NewPassword", error);
            }
            return BadRequest(ModelState);
        }


        return result.Status == ResetPasswordStatus.Succeeded ? NoContent() : Unauthorized(); ;
    }

    /// <summary>
    /// Logs out a user
    /// </summary>
    /// <response code="204">Success</response>
    /// <response code="401">Unauthorized user call</response>
    [HttpPost]
    [Route("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogOut(CancellationToken cancellationToken = default)
    {
        await _authService.LogOutAsync(cancellationToken);
        return NoContent();
    }

    private static ModelStateDictionary BuildModelStateFromErrors(IEnumerable<string>? errors)
    {
        var ms = new ModelStateDictionary();
        if (errors != null)
        {
            foreach (var e in errors)
            {
                ms.AddModelError(string.Empty, e);
            }
        }
        return ms;
    }
}
