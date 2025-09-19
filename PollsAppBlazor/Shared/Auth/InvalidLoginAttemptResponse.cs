namespace PollsAppBlazor.Shared.Auth;

public class InvalidLoginAttemptResponse
{
    public enum Reason
    {
        InvalidCredentials,
        EmailNotConfirmed,
        Banned,
        LockedOut
    }

    public required Reason FailureReason { get; set; }
    public required string ErrorMessage { get; set; }

    public static InvalidLoginAttemptResponse Default => new()
    {
        ErrorMessage = "Invalid login attempt.",
        FailureReason = Reason.InvalidCredentials
    };
}
