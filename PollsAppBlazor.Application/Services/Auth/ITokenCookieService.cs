namespace PollsAppBlazor.Application.Services.Auth;

public interface ITokenCookieService
{
    string? GetRefreshTokenFromCookie();

    void SetRefreshTokenCookie(string refreshToken);

    void DeleteRefreshTokenCookie();
}
