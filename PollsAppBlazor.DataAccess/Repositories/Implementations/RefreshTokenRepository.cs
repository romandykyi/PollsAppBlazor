using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.Application.Services.Auth.Tokens;
using PollsAppBlazor.DataAccess.Models;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;

namespace PollsAppBlazor.DataAccess.Repositories.Implementations;

public class RefreshTokenRepository(ApplicationDbContext dbContext) : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task CreateAsync(string userId, RefreshTokenValue token, CancellationToken cancellationToken)
    {
        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = userId,
            Persistent = token.Persistent,
            TokenValue = token.Value,
            ValidTo = token.ExpiresAt
        });
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<RefreshToken?> GetAsync(string tokenValue, CancellationToken cancellationToken)
    {
        return _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(x => x.TokenValue == tokenValue)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ReplaceAsync(string oldTokenValue, string newTokenValue, CancellationToken cancellationToken)
    {
        int updated = await _dbContext.RefreshTokens
            .Where(x => x.TokenValue == oldTokenValue)
            .ExecuteUpdateAsync(
                x => x.SetProperty(p => p.TokenValue, p => newTokenValue),
                cancellationToken
                );

        return updated > 0;
    }

    public async Task<bool> RevokeAsync(string tokenValue, CancellationToken cancellationToken)
    {
        int removed = await _dbContext.RefreshTokens
            .Where(x => x.TokenValue == tokenValue)
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
