using PollsAppBlazor.DataAccess.Models;

namespace PollsAppBlazor.DataAccess.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetAsync(Guid tokenId, CancellationToken cancellationToken);

    Task<RefreshToken> CreateAsync(string userId, RefreshToken token, CancellationToken cancellationToken);

    Task<bool> ReplaceAsync(Guid tokenId, string newTokenValue, CancellationToken cancellationToken);

    Task<bool> RevokeAsync(Guid tokenId, CancellationToken cancellationToken);

    Task<bool> RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken);
}
