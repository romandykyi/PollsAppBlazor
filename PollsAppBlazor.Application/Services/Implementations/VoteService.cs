using PollsAppBlazor.Application.Services.Interfaces;
using PollsAppBlazor.Application.Services.Results;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;

namespace PollsAppBlazor.Application.Services.Implementations;

public class VoteService(
    IVoteRepository voteRepository,
    IPollOptionRepository optionRepository,
    IPollRepository pollRepository
    ) : IVoteService
{
    private readonly IVoteRepository _voteRepository = voteRepository;
    private readonly IPollOptionRepository _optionRepository = optionRepository;
    private readonly IPollRepository _pollRepository = pollRepository;

    public Task<int?> GetVotedOptionAsync(int pollId, string userId)
    {
        return _voteRepository.GetVotedOptionAsync(pollId, userId);
    }

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
