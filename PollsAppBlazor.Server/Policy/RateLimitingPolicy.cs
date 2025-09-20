namespace PollsAppBlazor.Server.Policy;

public static class RateLimitingPolicy
{
    public const string LogInPolicy = "LogInPolicy";
    public const string RegisterPolicy = "RegisterPolicy";
    public const string ResetPasswordPolicy = "ResetPasswordPolicy";

    public const string CreatePolicy = "CreatePolicy";
    public const string EditPolicy = "EditPolicy";
    public const string VotePolicy = "VotePolicy";
}
