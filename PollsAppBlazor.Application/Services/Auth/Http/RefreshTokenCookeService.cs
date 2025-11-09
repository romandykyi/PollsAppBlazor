using Microsoft.AspNetCore.Http;

namespace PollsAppBlazor.Application.Services.Auth.Http;

public class RefreshTokenCookeService(IHttpContextAccessor httpContextAccessor) : IRefreshTokenCookieService
{
    public const string RefreshTokenCookieName = "refreshToken";

    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext;

    public string? GetRefreshTokenFromCookie()
    {
        if (_httpContext.Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken))
        {
            return refreshToken;
        }
        return null;
    }

    public void SetRefreshTokenCookie(string refreshToken, TimeSpan? maxAge)
    {
        CookieOptions options = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            MaxAge = maxAge
        };
        _httpContext.Response.Cookies.Append(RefreshTokenCookieName, refreshToken, options);
    }

    public void DeleteRefreshTokenCookie()
    {
        _httpContext.Response.Cookies.Delete(RefreshTokenCookieName);
    }
}
