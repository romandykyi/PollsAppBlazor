using PollsAppBlazor.Application.Services.Auth.Tokens;
using PollsAppBlazor.DataAccess.Models;

namespace PollsAppBlazor.DataAccess.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetAsync(string tokenValue, CancellationToken cancellationToken);

    Task CreateAsync(string userId, RefreshTokenValue token, CancellationToken cancellationToken);

    Task<bool> RevokeAsync(string tokenValue, CancellationToken cancellationToken);

    Task<bool> RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken);
}
