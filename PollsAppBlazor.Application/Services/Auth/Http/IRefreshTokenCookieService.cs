namespace PollsAppBlazor.Application.Services.Auth.Http;

public interface IRefreshTokenCookieService
{
    string? GetRefreshTokenFromCookie();

    void SetRefreshTokenCookie(string refreshToken, TimeSpan? maxAge);

    void DeleteRefreshTokenCookie();
}
