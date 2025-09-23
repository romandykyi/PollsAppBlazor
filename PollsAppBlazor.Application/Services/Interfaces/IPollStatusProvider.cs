using PollsAppBlazor.DataAccess.Aggregates;

namespace PollsAppBlazor.Application.Services.Interfaces;

public interface IPollStatusProvider
{
    /// <summary>
    /// Gets the poll's status by its ID. This operation is cached
    /// per request.
    /// </summary>
    /// <param name="pollId">ID of the poll to get status.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>
    /// The poll status or <see langword="null" /> if
    /// the poll doesn't exist.
    /// </returns>
    Task<PollStatus?> GetPollStatusAsync(int pollId, CancellationToken cancellationToken);
}
