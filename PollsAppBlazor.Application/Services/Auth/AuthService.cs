using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using PollsAppBlazor.Application.Emails;
using PollsAppBlazor.Application.Options;
using PollsAppBlazor.Application.Services.Communication.Interfaces;
using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Shared.Users;
using System.Text;

namespace PollsAppBlazor.Application.Services.Auth;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IUserStore<ApplicationUser> userStore,
    SignInManager<ApplicationUser> signInManager,
    IEmailService emailService,
    IOptions<UriOptions> uriOptions
    ) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUserStore<ApplicationUser> _userStore = userStore;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IUserEmailStore<ApplicationUser> _emailStore = (IUserEmailStore<ApplicationUser>)userStore;
    private readonly IEmailService _emailService = emailService;
    private readonly IOptions<UriOptions> _uriOptions = uriOptions;

    public async Task<LoginResult> LogInAsync(UserLoginDto loginDto, CancellationToken cancellationToken = default)
    {
        if (loginDto is null) return LoginResult.Fail(LoginFailureReason.InvalidCredentials, "Login data is null.");

        var user = await _signInManager.UserManager.FindByEmailAsync(loginDto.EmailOrUsername) ??
                   await _signInManager.UserManager.FindByNameAsync(loginDto.EmailOrUsername);

        if (user == null) return LoginResult.Fail(LoginFailureReason.UserNotFound, "Invalid login attempt.");
        if (user.IsDeleted) return LoginResult.Fail(LoginFailureReason.UserDeleted, "Invalid login attempt.");
        if (!user.EmailConfirmed)
        {
            // Only disclose if the password is correct
            if (await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return LoginResult.Fail(LoginFailureReason.EmailNotConfirmed, "You must have a confirmed email to log in.");
            }
            return LoginResult.Fail(LoginFailureReason.InvalidCredentials, "Invalid login attempt.");
        }

        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded) return LoginResult.Success();

        // Only disclose locked-out if the password was correct
        if (result.IsLockedOut && await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return LoginResult.Fail(LoginFailureReason.LockedOut, "User account locked out, please try again.");
        }

        return LoginResult.Fail(LoginFailureReason.InvalidCredentials, "Invalid login attempt.");
    }

    public async Task<RegisterResult> RegisterAsync(UserRegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        var newUser = Activator.CreateInstance<ApplicationUser>()!;
        await _userStore.SetUserNameAsync(newUser, registerDto.Username, CancellationToken.None);
        await _emailStore.SetEmailAsync(newUser, registerDto.Email, CancellationToken.None);

        var result = await _userManager.CreateAsync(newUser, registerDto.Password);
        if (!result.Succeeded)
        {
            var errs = result.Errors.Select(e => e.Description).ToArray();
            return RegisterResult.Failed(errs, emailSent: false);
        }

        // Generate confirmation and send email
        string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
        string urlToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));
        string confirmationLink = string.Format(_uriOptions.Value.ConfirmEmailUri, newUser.Id, urlToken);

        bool sendResult = await _emailService.SendAsync(
            registerDto.Email,
            "Please Confirm Your Email for Polls App Blazor",
            string.Format(AuthEmails.ConfirmationEmailBody, registerDto.Username, confirmationLink),
            cancellationToken);

        if (!sendResult)
        {
            return RegisterResult.Failed(["Failed to send confirmation email."], emailSent: false);
        }

        return RegisterResult.Success(emailSent: true);
    }

    public async Task<ConfirmEmailResult> ConfirmEmailAsync(string userId, string encodedToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId)) return ConfirmEmailResult.Failed(ConfirmEmailStatus.UserNotFound, "UserId is required.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.IsDeleted) return ConfirmEmailResult.Failed(ConfirmEmailStatus.UserNotFound, "User not found.");
        if (user.EmailConfirmed) return ConfirmEmailResult.Failed(ConfirmEmailStatus.AlreadyConfirmed, "Email is already confirmed.");

        string decodedToken;
        try
        {
            decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken));
        }
        catch (FormatException)
        {
            return ConfirmEmailResult.Failed(ConfirmEmailStatus.InvalidToken, "Token is not valid base64 string.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        return result.Succeeded ? ConfirmEmailResult.Succeeded() : ConfirmEmailResult.Failed(ConfirmEmailStatus.Failed, "Failed to confirm email.");
    }

    public async Task<InitiateResetPasswordResult> InitiatePasswordResetAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return InitiateResetPasswordResult.Failed(InitiateResetPasswordStatus.UserNotFound);

        var user = await _signInManager.UserManager.FindByEmailAsync(email);
        if (user == null || user.IsDeleted) return InitiateResetPasswordResult.Failed(InitiateResetPasswordStatus.UserNotFound);

        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        string urlToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        string resetLink = string.Format(_uriOptions.Value.ResetPasswordUri, user.Id, urlToken);

        bool sendResult = await _emailService.SendAsync(user.Email!, "Reset Your Password for Polls App Blazor",
            string.Format(AuthEmails.ResetPasswordEmailBody, user.UserName, resetLink),
            cancellationToken);

        if (!sendResult) return InitiateResetPasswordResult.Failed(InitiateResetPasswordStatus.EmailSendFailed, "Failed to send reset email.");
        return InitiateResetPasswordResult.Succeeded();
    }

    public async Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default)
    {
        if (dto is null) return ResetPasswordResult.Failed(ResetPasswordStatus.Failed, "Reset data is null.");
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null || user.IsDeleted) return ResetPasswordResult.Failed(ResetPasswordStatus.UserNotFound);

        string token;
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
        }
        catch (FormatException)
        {
            return ResetPasswordResult.Failed(ResetPasswordStatus.InvalidToken, "Token is not valid base64 string.");
        }

        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
        if (!result.Succeeded)
        {
            var passwordErrors = result.Errors
                .Where(e => e.Description.Contains("Password", StringComparison.OrdinalIgnoreCase))
                .Select(e => e.Description)
                .ToArray();

            if (passwordErrors.Length > 0)
            {
                return ResetPasswordResult.Failed(ResetPasswordStatus.PasswordRequirementsFailed, "Password does not meet requirements.", passwordErrors);
            }

            // Generic failure
            return ResetPasswordResult.Failed(ResetPasswordStatus.Failed, "Failed to reset password.", result.Errors.Select(e => e.Description));
        }

        return ResetPasswordResult.Succeeded();
    }

    public async Task LogOutAsync(CancellationToken cancellationToken = default)
    {
        await _signInManager.SignOutAsync();
    }

}
