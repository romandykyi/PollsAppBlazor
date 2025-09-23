using PollsAppBlazor.Application.Services.Results;

namespace PollsAppBlazor.Application.Services.Interfaces;

/// <summary>
/// Service for managing user's favorite polls.
/// </summary>
public interface IFavoriteService
{
    /// <summary>
    /// Sets the poll's favorite state to <paramref name="value"/> for the current user.
    /// </summary>
    /// <param name="pollId">ID of the poll.</param>
    /// <param name="userId">ID of the caller.</param>
    /// <param name="value">A boolean value to set.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>The result of the operation.</returns>
    Task<SetFavoriteStateResult> SetPollFavoriteState(int pollId, string userId, bool value, CancellationToken cancellationToken);
}
