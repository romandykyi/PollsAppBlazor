using PollsAppBlazor.Application.Services.Results;

namespace PollsAppBlazor.Application.Services.Interfaces;

/// <summary>
/// Service for managing votes on polls.
/// </summary>
public interface IVoteService
{
    /// <summary>
    /// Gets ID of an option which was voted by the user.
    /// </summary>
    /// <param name="pollId">ID of the poll.</param>
    /// <param name="userId">ID of the user.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>
    /// ID of the voted option, or <see langword="null" /> if user didn't vote on this poll.
    /// </returns>
    Task<int?> GetVotedOptionAsync(int pollId, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a user vote for the option.
    /// </summary>
    /// <param name="optionId">ID of the option.</param>
    /// <param name="userId">ID of the voter.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>
    /// A <see cref="VoteServiceResult"/> describing the result of the voting attempt.
    /// </returns>
    Task<VoteServiceResult> VoteAsync(int optionId, string userId, CancellationToken cancellationToken);
}
