using PollsAppBlazor.Shared.Users;

namespace PollsAppBlazor.Application.Services.Auth;

public interface IAuthService
{
    /// <summary>
    /// Attempts to log in a user.
    /// </summary>
    Task<LoginResult> LogInAsync(UserLoginDto loginDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user.
    /// </summary>
    Task<RegisterResult> RegisterAsync(UserRegisterDto registerDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms an email using the base64-url-encoded token (as received from querystring).
    /// </summary>
    Task<ConfirmEmailResult> ConfirmEmailAsync(string userId, string encodedToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates password reset by generating token and sending email.
    /// </summary>
    Task<InitiateResetPasswordResult> InitiatePasswordResetAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets a user's password using a base64-url-encoded token in ResetPasswordDto.Token.
    /// </summary>
    Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out the current user (clears cookies / auth session).
    /// </summary>
    Task LogOutAsync(CancellationToken cancellationToken = default);
}
