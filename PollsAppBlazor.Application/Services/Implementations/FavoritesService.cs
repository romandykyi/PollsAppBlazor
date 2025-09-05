using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Application.Services.Implementations;

public class FavoritesService(IFavoriteRepository favoritesRepository, ApplicationDbContext context)
{
    private readonly IFavoriteRepository _favoriteRepository = favoritesRepository;
    private readonly ApplicationDbContext _context = context;

    /// <summary>
    /// Get a Poll favorite status for the user.
    /// </summary>
    /// <remarks>
    /// This method doesn't check whether Poll exists.
    /// </remarks>
    /// <param name="pollId">ID of the Poll to be checked</param>
    /// <param name="userId">ID the user</param>
    public async Task<FavoriteDto> GetFavorite(int pollId, string userId)
    {
        bool isFavorite = await _favoriteRepository.ExistsAsync(pollId, userId);
        return new()
        {
            PollId = pollId,
            IsFavorite = isFavorite
        };
    }

    /// <summary>
    /// Add a Poll to favorites for the user.
    /// </summary>
    /// <remarks>
    /// If the Poll is already in favorites then nothing will happen.
    /// </remarks>
    /// <param name="pollId">ID of the Poll to be addded to favorites</param>
    /// <param name="userId">ID the user</param>
    /// <returns>
    /// <see langword="true" /> if the Poll was added to favorites or was already in favorites;
    /// otherwise <see langword="false" /> if the Poll was not found.
    /// </returns>
    public async Task<bool> AddToFavoritesAsync(int pollId, string userId)
    {
        // If poll doesn't exist
        if (!await _context.Polls
            .AsNoTracking()
            .AnyAsync(p => p.Id == pollId))
        {
            return false;
        }

        await _favoriteRepository.AddAsync(pollId, userId);
        return true;
    }

    /// <summary>
    /// Remove a Poll from favorites for the user.
    /// </summary>
    /// <param name="pollId">ID of the Poll to be removed from favorites</param>
    /// <param name="userId">ID the user</param>
    /// <returns>
    /// <see langword="true" /> if the Poll was successfully removed from favorites;
    /// otherwise <see langword="false" /> if Poll wasn't in favorites.
    /// </returns>
    public Task<bool> RemoveFromFavoritesAsync(int pollId, string userId)
    {
        return _favoriteRepository.RemoveAsync(pollId, userId);
    }
}
