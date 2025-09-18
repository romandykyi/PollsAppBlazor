namespace PollsAppBlazor.Server.Policy;

public static class RateLimitingPolicy
{
    public const string LogInShortPolicy = "LogInShortPolicy";
    public const string LogInLongPolicy = "LogInLongPolicy";

    public const string RegisterShortPolicy = "RegisterShortPolicy";
    public const string RegisterLongPolicy = "RegisterLongPolicy";

    public const string CreateShortPolicy = "CreateShortPolicy";
    public const string CreateLongPolicy = "CreateLongPolicy";

    public const string EditShortPolicy = "EditShortPolicy";
    public const string EditLongPolicy = "EditLongPolicy";

    public const string VotePolicy = "VotePolicy";
}
