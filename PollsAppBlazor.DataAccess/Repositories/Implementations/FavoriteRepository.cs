using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;
using PollsAppBlazor.Server.DataAccess.Models;

namespace PollsAppBlazor.DataAccess.Repositories.Implementations;

public class FavoriteRepository(ApplicationDbContext dbContext) : IFavoriteRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    /// <summary>
    /// Checks whether the poll <paramref name="pollId"/> is marked as favorite by
    /// user <paramref name="userId"/>.
    /// </summary>
    /// <param name="pollId">Poll's ID.</param>
    /// <param name="userId">User's ID.</param>
    /// <returns>
    /// <see langword="true"/> if the poll is marked as favorite;
    /// <see langword="false"/> if the favorite doesn't exist.
    /// </returns>
    public Task<bool> ExistsAsync(int pollId, string userId)
    {
        return _dbContext.Favorites
            .AsNoTracking()
            .AnyAsync(f => f.PollId == pollId && f.UserId == userId);
    }

    /// <summary>
    /// Marks poll <paramref name="pollId"/> as favorite for the user <paramref name="userId"/>.
    /// </summary>
    /// <param name="pollId">Poll's ID.</param>
    /// <param name="userId">User's ID.</param>
    /// <returns>
    /// <see langword="true"/> if the poll was marked as favorite;
    /// <see langword="false"/> if the poll was already marked as favorite.
    /// </returns>
    public async Task<bool> AddAsync(int pollId, string userId)
    {
        // If isn't already in favorites
        if (!await ExistsAsync(pollId, userId))
        {
            Favorite favorite = new()
            {
                PollId = pollId,
                UserId = userId
            };

            _dbContext.Add(favorite);
            await _dbContext.SaveChangesAsync();
        }

        return true;
    }

    /// <summary>
    /// Removes poll <paramref name="pollId"/> from favorites for the user <paramref name="userId"/>.
    /// </summary>
    /// <param name="pollId">Poll's ID.</param>
    /// <param name="userId">User's ID.</param>
    /// <returns>
    /// <see langword="true"/> if the poll was removed;
    /// <see langword="false"/> if the poll wasn't marked as favorite.
    /// </returns>
    public async Task<bool> RemoveAsync(int pollId, string userId)
    {
        int entriesRemoved = await _dbContext.Favorites
            .Where(f => f.PollId == pollId && f.UserId == userId)
            .ExecuteDeleteAsync();

        return entriesRemoved > 0;
    }
}
