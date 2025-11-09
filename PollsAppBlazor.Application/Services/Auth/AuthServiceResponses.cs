namespace PollsAppBlazor.Application.Services.Auth;

// Login
public enum LoginFailureReason
{
    None,
    InvalidCredentials,
    LockedOut,
    EmailNotConfirmed,
    UserDeleted,
    UserNotFound
}
public class LoginResult
{
    public bool Succeeded { get; init; }
    public LoginFailureReason FailureReason { get; init; } = LoginFailureReason.None;
    public string? AccessToken { get; init; }
    public string? ErrorMessage { get; init; }

    public static LoginResult Success(string accessToken) =>
        new() { Succeeded = true, AccessToken = accessToken };
    public static LoginResult Fail(LoginFailureReason reason, string? message = null) =>
        new() { Succeeded = false, FailureReason = reason, ErrorMessage = message };
}

// Refresh
public enum RefreshFailureReason
{
    None,
    InvalidToken,
    ExpiredToken,
    UserNotFound,
    UserDeleted
}
public class RefreshResult
{
    public bool Succeeded { get; init; }
    public RefreshFailureReason FailureReason { get; init; } = RefreshFailureReason.None;
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public string? ErrorMessage { get; init; }

    public static RefreshResult Success(string accessToken) =>
        new() { Succeeded = true, AccessToken = accessToken };
    public static RefreshResult Fail(RefreshFailureReason reason, string? message = null) =>
        new() { Succeeded = false, FailureReason = reason, ErrorMessage = message };
}

// Register
public enum RegisterStatus
{
    Succeeded,
    Failed,
}
public class RegisterResult
{
    public RegisterStatus Status { get; init; }
    public IEnumerable<string>? Errors { get; init; }
    public bool EmailSent { get; init; }
    public static RegisterResult Success(bool emailSent = true) => new() { Status = RegisterStatus.Succeeded, EmailSent = emailSent };
    public static RegisterResult Failed(IEnumerable<string> errors, bool emailSent = false) => new() { Status = RegisterStatus.Failed, Errors = errors, EmailSent = emailSent };
}

public enum ConfirmEmailStatus
{
    Succeeded,
    AlreadyConfirmed,
    InvalidToken,
    UserNotFound,
    Failed
}

public class ConfirmEmailResult
{
    public ConfirmEmailStatus Status { get; init; }
    public string? ErrorMessage { get; init; }
    public static ConfirmEmailResult Succeeded() => new() { Status = ConfirmEmailStatus.Succeeded };
    public static ConfirmEmailResult Failed(ConfirmEmailStatus status, string? msg = null) => new() { Status = status, ErrorMessage = msg };
}

public enum InitiateResetPasswordStatus
{
    Succeeded,
    UserNotFound,
    EmailSendFailed
}
public class InitiateResetPasswordResult
{
    public InitiateResetPasswordStatus Status { get; init; }
    public string? ErrorMessage { get; init; }
    public static InitiateResetPasswordResult Succeeded() => new() { Status = InitiateResetPasswordStatus.Succeeded };
    public static InitiateResetPasswordResult Failed(InitiateResetPasswordStatus s, string? msg = null) => new() { Status = s, ErrorMessage = msg };
}

public enum ResetPasswordStatus
{
    Succeeded,
    InvalidToken,
    PasswordRequirementsFailed,
    UserNotFound,
    Failed
}
public class ResetPasswordResult
{
    public ResetPasswordStatus Status { get; init; }
    public IEnumerable<string>? Errors { get; init; }
    public string? ErrorMessage { get; init; }

    public static ResetPasswordResult Succeeded() => new() { Status = ResetPasswordStatus.Succeeded };
    public static ResetPasswordResult Failed(ResetPasswordStatus status, string? msg = null, IEnumerable<string>? errors = null) =>
        new() { Status = status, ErrorMessage = msg, Errors = errors };
}
