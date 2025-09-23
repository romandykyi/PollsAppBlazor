using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Application.Services.Interfaces;

/// <summary>
/// Service for retrieving user-related polls.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets a page of polls created by the specified user.
    /// </summary>
    /// <param name="parameters">Parameters to use for the polls retrieval.</param>
    /// <param name="userId">User's ID.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>
    /// Polls created by the user with ID <paramref name="userId"/>.
    /// If user does not exist, an empty page is returned.
    /// </returns>
    Task<PollsPage> GetPollsAsync(PollsPagePaginationParameters parameters, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a page of favorite polls of the specified user.
    /// </summary>
    /// <param name="parameters">Parameters to use for the polls retrieval.</param>
    /// <param name="userId">User's ID.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>
    /// Polls that were marked as favorite by the user with ID <paramref name="userId"/>.
    /// If user does not exist, an empty page is returned.
    /// </returns>
    Task<PollsPage> GetFavoritePollsAsync(PollsPagePaginationParameters parameters, string userId, CancellationToken cancellationToken);
}
