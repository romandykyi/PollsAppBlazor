namespace PollsAppBlazor.DataAccess.Repositories.Interfaces;

public interface IVoteRepository
{
    /// <summary>
    /// Gets an ID of an option which was voted by the user.
    /// </summary>
    /// <param name="pollId">ID of the poll.</param>
    /// <param name="userId">ID of the user.</param>
    /// <returns>
    /// ID of the voted by the user option, or
    /// <see langword="null" /> if user didn't vote on this poll.
    /// </returns>
    Task<int?> GetVotedOptionAsync(int pollId, string userId);

    /// <summary>
    /// Adds a vote for the option.
    /// </summary>
    /// <param name="pollId">ID of the poll which contains this option.</param>
    /// <param name="optionId">ID of the option.</param>
    /// <param name="userId">ID of the user who votes.</param>
    Task VoteAsync(int pollId, int optionId, string userId);
}
