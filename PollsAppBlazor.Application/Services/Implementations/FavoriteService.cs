using PollsAppBlazor.Application.Services.Interfaces;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;

namespace PollsAppBlazor.Application.Services.Implementations;

public class FavoriteService(IFavoriteRepository favoritesRepository) : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository = favoritesRepository;

    public Task<bool> AddToFavoritesAsync(int pollId, string userId)
    {
        return _favoriteRepository.AddAsync(pollId, userId);
    }

    public Task<bool> RemoveFromFavoritesAsync(int pollId, string userId)
    {
        return _favoriteRepository.RemoveAsync(pollId, userId);
    }
}
