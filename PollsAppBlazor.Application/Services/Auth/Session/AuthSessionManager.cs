using Microsoft.AspNetCore.Identity;
using PollsAppBlazor.Application.Services.Auth.Http;
using PollsAppBlazor.Application.Services.Auth.Tokens;
using PollsAppBlazor.Server.DataAccess.Models;

namespace PollsAppBlazor.Application.Services.Auth.Session;

public class AuthSessionManager(
    IRefreshTokenCookieService cookieService,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    UserManager<ApplicationUser> userManager
    ) : IAuthSessionManager
{
    private readonly IRefreshTokenCookieService _cookieService = cookieService;
    private readonly IAccessTokenService _accessTokenService = accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;

    private async Task<AuthSession> InternalStartSessionAsync(ApplicationUser user, RefreshTokenValue refreshToken)
    {
        var roles = await userManager.GetRolesAsync(user);

        string accessToken = _accessTokenService.GenerateAccessToken(user, roles);
        _cookieService.SetRefreshTokenCookie(
            refreshToken.Value,
            refreshToken.Persistent ? refreshToken.ExpiresAt : null
            );

        return new AuthSession(accessToken, refreshToken.Value);
    }

    public async Task<AuthSession> StartSessionAsync(ApplicationUser user, bool persistent, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _refreshTokenService
            .GenerateAsync(user.Id, persistent, cancellationToken) ??
            throw new InvalidOperationException("Failed to generate refresh token - user does not exist.");

        return await InternalStartSessionAsync(user, refreshToken);
    }

    public Task<bool> InvalidateCurrentSessionAsync(string userId, CancellationToken cancellationToken = default)
    {
        string? refreshToken = _cookieService.GetRefreshTokenFromCookie();
        if (refreshToken == null)
        {
            return Task.FromResult(false);
        }

        return _refreshTokenService.RevokeAsync(userId, refreshToken, cancellationToken);
    }

    public async Task<AuthSession?> ResumeCurrentSessionAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        string? refreshToken = _cookieService.GetRefreshTokenFromCookie();
        if (refreshToken == null)
        {
            return null;
        }

        var updatedToken = await _refreshTokenService.ValidateAsync(user.Id, refreshToken, cancellationToken);
        if (updatedToken == null)
        {
            return null;
        }

        return await InternalStartSessionAsync(user, updatedToken);
    }
}
