using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.Application.Services.Auth.Tokens;
using PollsAppBlazor.DataAccess.Models;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;

namespace PollsAppBlazor.DataAccess.Repositories.Implementations;

public class RefreshTokenRepository(ApplicationDbContext dbContext) : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<RefreshTokenValue?> GetAsync(string userId, string tokenValue, CancellationToken cancellationToken)
    {
        return _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.TokenValue == tokenValue)
            .Select(token => new RefreshTokenValue(token.TokenValue, token.Persistent, token.ValidTo))
            .SingleOrDefaultAsync(cancellationToken);
    }

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

    public async Task<bool> RevokeAsync(string userId, string tokenValue, CancellationToken cancellationToken)
    {
        int removed = await _dbContext.RefreshTokens
            .Where(x => x.UserId == userId && x.TokenValue == tokenValue)
            .ExecuteDeleteAsync(cancellationToken);

        return removed > 0;
    }
}
