using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;
using PollsAppBlazor.Server.DataAccess.Models;

namespace PollsAppBlazor.DataAccess.Repositories.Implementations;

public class FavoriteRepository(ApplicationDbContext dbContext) : IFavoriteRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly Dictionary<int, bool> _existsLookup = [];

    public async Task<bool> ExistsAsync(int pollId, string userId, CancellationToken cancellationToken)
    {
        if (_existsLookup.TryGetValue(pollId, out var exists))
        {
            return exists;
        }

        bool result = await _dbContext.Favorites
            .AsNoTracking()
            .AnyAsync(f => f.PollId == pollId && f.UserId == userId, cancellationToken);

        _existsLookup[pollId] = result;
        return result;
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

            _existsLookup[pollId] = true;
        }

        return true;
    }

    public async Task<bool> RemoveAsync(int pollId, string userId, CancellationToken cancellationToken)
    {
        int entriesRemoved = await _dbContext.Favorites
            .Where(f => f.PollId == pollId && f.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        if (entriesRemoved > 0)
        {
            _existsLookup[pollId] = false;
            return true;
        }
        return false;
    }
}
