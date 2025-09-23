using PollsAppBlazor.Application.Services.Interfaces;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.DataAccess.Repositories.Options;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Application.Services.Implementations;

public class UserService(IPollRepository pollRepository) : IUserService
{
    private readonly IPollRepository _pollRepository = pollRepository;

    public Task<PollsPage> GetPollsAsync(PollsPagePaginationParameters parameters, string userId, CancellationToken cancellationToken)
    {
        return _pollRepository.GetPollsPageAsync(new PollsRetrievalOptions(
            Parameters: parameters,
            CreatorId: userId,
            FavoritesOfUserId: null),
            cancellationToken
            );
    }

    public Task<PollsPage> GetFavoritePollsAsync(PollsPagePaginationParameters parameters, string userId, CancellationToken cancellationToken)
    {
        return _pollRepository.GetPollsPageAsync(new PollsRetrievalOptions(
            Parameters: parameters,
            CreatorId: null,
            FavoritesOfUserId: userId),
            cancellationToken
            );
    }
}
