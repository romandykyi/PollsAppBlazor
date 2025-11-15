namespace PollsAppBlazor.Application.Services.Auth.Http;

public interface IRefreshTokenCookieService
{
    string? GetRefreshTokenFromCookie();

    void SetRefreshTokenCookie(string refreshToken, DateTime? expiresAt);

    void DeleteRefreshTokenCookie();
}
