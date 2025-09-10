using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.DataAccess.Repositories.Options;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Application.Services.Implementations;

public class UserService(IPollRepository pollRepository)
{
    private readonly IPollRepository _pollRepository = pollRepository;

    /// <summary>
    /// Gets a page of polls created by the specified user.
    /// </summary>
    /// <param name="parameters">Parameters to use for the polls retrieval.</param>
    /// <param name="userId">User's ID.</param>
    /// <returns>
    /// Polls created by the user with ID <paramref name="userId"/>.
    /// If user does not exist, an empty page is returned.
    /// </returns>
    public Task<PollsPage> GetPollsAsync(PollsPagePaginationParameters parameters, string userId)
    {
        return _pollRepository.GetPollsPageAsync(new PollsRetrievalOptions(
            Parameters: parameters,
            CreatorId: userId,
            FavoritesOfUserId: null)
            );
    }

    /// <summary>
    /// Gets a page of favorite polls of the specified user.
    /// </summary>
    /// <param name="parameters">Parameters to use for the polls retrieval.</param>
    /// <param name="userId">User's ID.</param>
    /// <returns>
    /// Polls that were marked as favorite by the user with ID <paramref name="userId"/>.
    /// If user does not exist, an empty page is returned.
    /// </returns>
    public Task<PollsPage> GetFavoritePollsAsync(PollsPagePaginationParameters parameters, string userId)
    {
        return _pollRepository.GetPollsPageAsync(new PollsRetrievalOptions(
            Parameters: parameters,
            CreatorId: null,
            FavoritesOfUserId: userId)
            );
    }
}
