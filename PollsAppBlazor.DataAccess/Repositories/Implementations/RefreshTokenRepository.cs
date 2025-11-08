using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.DataAccess.Dto;
using PollsAppBlazor.DataAccess.Models;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;

namespace PollsAppBlazor.DataAccess.Repositories.Implementations;

public class RefreshTokenRepository(ApplicationDbContext dbContext) : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<RefreshTokenValidationDto?> GetAsync(string userId, string tokenValue, CancellationToken cancellationToken)
    {
        return _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.TokenValue == tokenValue)
            .Select(token => new RefreshTokenValidationDto(token.TokenId, token.ValidTo))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task CreateAsync(string userId, string tokenValue, DateTime validTo, CancellationToken cancellationToken)
    {
        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = userId,
            TokenValue = tokenValue,
            ValidTo = validTo
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
