using PollsAppBlazor.DataAccess.Repositories.Interfaces;

namespace PollsAppBlazor.Application.Services.Implementations;

public class FavoritesService(IFavoriteRepository favoritesRepository)
{
    private readonly IFavoriteRepository _favoriteRepository = favoritesRepository;

    /// <summary>
    /// Adds a poll to favorites for the user.
    /// </summary>
    /// <remarks>
    /// If the Poll is already in favorites, then nothing will happen.
    /// </remarks>
    /// <param name="pollId">ID of the Poll to be addded to favorites.</param>
    /// <param name="userId">ID the user.</param>
    /// <returns>
    /// <see langword="true" /> if the Poll was added to favorites or was already in favorites;
    /// otherwise <see langword="false" /> if the Poll was not found.
    /// </returns>
    public Task<bool> AddToFavoritesAsync(int pollId, string userId)
    {
        return _favoriteRepository.AddAsync(pollId, userId);
    }

    /// <summary>
    /// Removes a poll from favorites for the user.
    /// </summary>
    /// <param name="pollId">ID of the Poll to be removed from favorites.</param>
    /// <param name="userId">ID the user.</param>
    /// <returns>
    /// <see langword="true" /> if the Poll was successfully removed from favorites;
    /// otherwise <see langword="false" /> if Poll wasn't in favorites.
    /// </returns>
    public Task<bool> RemoveFromFavoritesAsync(int pollId, string userId)
    {
        return _favoriteRepository.RemoveAsync(pollId, userId);
    }
}
