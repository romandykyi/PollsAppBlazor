using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.DataAccess.Models;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;

namespace PollsAppBlazor.DataAccess.Repositories.Implementations;

public class RefreshTokenRepository(ApplicationDbContext dbContext) : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<RefreshToken> CreateAsync(string userId, RefreshToken token, CancellationToken cancellationToken)
    {
        _dbContext.RefreshTokens.Add(token);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return token;
    }

    public Task<RefreshToken?> GetAsync(Guid tokenId, CancellationToken cancellationToken)
    {
        return _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(x => x.Id == tokenId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ReplaceAsync(Guid tokenId, string newTokenValue, CancellationToken cancellationToken)
    {
        int updated = await _dbContext.RefreshTokens
            .Where(x => x.Id == tokenId)
            .ExecuteUpdateAsync(
                x => x.SetProperty(p => p.TokenHash, p => newTokenValue),
                cancellationToken
                );

        return updated > 0;
    }

    public async Task<bool> RevokeAsync(Guid tokenId, CancellationToken cancellationToken)
    {
        int removed = await _dbContext.RefreshTokens
            .Where(x => x.Id == tokenId)
            .ExecuteDeleteAsync(cancellationToken);

        return removed > 0;
    }

    public async Task<bool> RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken)
    {
        int removed = await _dbContext.RefreshTokens
            .Where(x => x.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        return removed > 0;
    }
}
