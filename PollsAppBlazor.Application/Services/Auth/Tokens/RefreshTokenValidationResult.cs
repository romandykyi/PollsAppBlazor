namespace PollsAppBlazor.Application.Services.Auth.Tokens;

public class RefreshTokenValidationResult
{
    public bool Succeeded { get; init; }
    public RefreshFailureReason FailureReason { get; init; } = RefreshFailureReason.None;

    public RefreshTokenValue? UpdatedToken { get; init; } = null;
    public string? UserId { get; init; } = null;

    public static RefreshTokenValidationResult Success(RefreshTokenValue updatedToken, string userId) =>
        new() { Succeeded = true, UserId = userId, UpdatedToken = updatedToken };

    public static RefreshTokenValidationResult Fail(RefreshFailureReason reason) =>
        new() { Succeeded = false, FailureReason = reason };
}
