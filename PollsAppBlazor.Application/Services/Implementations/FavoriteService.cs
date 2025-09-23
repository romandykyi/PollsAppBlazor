using PollsAppBlazor.Application.Services.Interfaces;
using PollsAppBlazor.Application.Services.Results;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;

namespace PollsAppBlazor.Application.Services.Implementations;

public class FavoriteService(IFavoriteRepository favoritesRepository, IPollStatusProvider pollStatusProvider)
    : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository = favoritesRepository;
    private readonly IPollStatusProvider _pollStatusProvider = pollStatusProvider;
    public async Task<SetFavoriteStateResult> SetPollFavoriteState(int pollId, string userId, bool value, CancellationToken cancellationToken)
    {
        var pollStatus = await _pollStatusProvider.GetPollStatusAsync(pollId, cancellationToken);
        if (pollStatus == null) return SetFavoriteStateResult.NotFound;
        if (pollStatus.IsDeleted) return SetFavoriteStateResult.Deleted;

        bool isInFavorites = await _favoriteRepository.ExistsAsync(pollId, userId, cancellationToken);
        // Nothing to change
        if (isInFavorites == value) return SetFavoriteStateResult.Success;

        if (value)
        {
            await _favoriteRepository.AddAsync(pollId, userId, cancellationToken);
        }
        else
        {
            await _favoriteRepository.RemoveAsync(pollId, userId, cancellationToken);
        }

        return SetFavoriteStateResult.Success;
    }
}
