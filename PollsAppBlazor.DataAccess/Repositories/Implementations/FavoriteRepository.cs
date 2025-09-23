using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;
using PollsAppBlazor.Server.DataAccess.Models;

namespace PollsAppBlazor.DataAccess.Repositories.Implementations;

public class FavoriteRepository(ApplicationDbContext dbContext) : IFavoriteRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<bool> ExistsAsync(int pollId, string userId, CancellationToken cancellationToken)
    {
        return _dbContext.Favorites
            .AsNoTracking()
            .AnyAsync(f => f.PollId == pollId && f.UserId == userId, cancellationToken);
    }

    public async Task<bool> AddAsync(int pollId, string userId, CancellationToken cancellationToken)
    {
        // If isn't already in favorites
        if (!await ExistsAsync(pollId, userId, cancellationToken))
        {
            Favorite favorite = new()
            {
                PollId = pollId,
                UserId = userId
            };

            _dbContext.Add(favorite);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    public async Task<bool> RemoveAsync(int pollId, string userId, CancellationToken cancellationToken)
    {
        int entriesRemoved = await _dbContext.Favorites
            .Where(f => f.PollId == pollId && f.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        return entriesRemoved > 0;
    }
}
