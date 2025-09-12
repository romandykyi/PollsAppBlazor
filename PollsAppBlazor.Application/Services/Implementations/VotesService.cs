using PollsAppBlazor.Application.Services.Results;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;

namespace PollsAppBlazor.Application.Services.Implementations;

public class VotesService(
    IVoteRepository voteRepository,
    IPollOptionRepository optionRepository,
    IPollRepository pollRepository
    )
{
    private readonly IVoteRepository _voteRepository = voteRepository;
    private readonly IPollOptionRepository _optionRepository = optionRepository;
    private readonly IPollRepository _pollRepository = pollRepository;

    /// <summary>
    /// Gets ID of an option which was voted by the user
    /// </summary>
    /// <param name="pollId">ID of the poll.</param>
    /// <param name="userId">ID of the user.</param>
    /// <returns>
    /// ID of the voted option, or <see langword="null" /> if user didn't vote on this poll.
    /// </returns>
    public Task<int?> GetVotedOptionAsync(int pollId, string userId)
    {
        return _voteRepository.GetVotedOptionAsync(pollId, userId);
    }

    /// <summary>
    /// Adds a user vote for the option
    /// </summary>
    /// <param name="optionId">ID of the option.</param>
    /// <param name="userId">ID of the voter.</param>
    public async Task<VoteServiceResult> VoteAsync(int optionId, string userId)
    {
        // Check whether option has Poll assigned to it
        int? pollId = await _optionRepository.GetOptionPollIdAsync(optionId);
        if (pollId == null)
        {
            return VoteServiceResult.PollNotFound;
        }
        // Make sure that poll is active
        var pollStatus = await _pollRepository.GetPollStatusAsync(pollId.Value);
        if (pollStatus == null)
        {
            return VoteServiceResult.PollNotFound;
        }
        if (pollStatus.IsExpired)
        {
            return VoteServiceResult.PollExpired;
        }
        // Make sure that user isn't voting twice
        if (await _voteRepository.GetVotedOptionAsync(pollId.Value, userId) != null)
        {
            return VoteServiceResult.AlreadyVoted;
        }

        // Vote
        await _voteRepository.AddVoteAsync(pollId.Value, optionId, userId);

        return VoteServiceResult.Success;
    }
}
