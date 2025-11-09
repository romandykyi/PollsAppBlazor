namespace PollsAppBlazor.Application.Services.Auth.Tokens;

public record RefreshTokenValue(string Value, bool Persistent, DateTime ExpiresAt);
