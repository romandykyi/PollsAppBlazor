using PollsAppBlazor.Application.Services.Auth.Tokens;

namespace PollsAppBlazor.DataAccess.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenValue?> GetAsync(string userId, string tokenValue, CancellationToken cancellationToken);

    Task CreateAsync(string userId, RefreshTokenValue token, CancellationToken cancellationToken);

    Task<bool> RevokeAsync(string userId, string tokenValue, CancellationToken cancellationToken);
}
