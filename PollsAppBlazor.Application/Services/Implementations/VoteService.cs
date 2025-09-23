using PollsAppBlazor.Application.Services.Interfaces;
using PollsAppBlazor.Application.Services.Results;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;

namespace PollsAppBlazor.Application.Services.Implementations;

public class VoteService(
    IVoteRepository voteRepository,
    IPollOptionRepository optionRepository,
    IPollStatusProvider pollStatusProvider
    ) : IVoteService
{
    private readonly IVoteRepository _voteRepository = voteRepository;
    private readonly IPollOptionRepository _optionRepository = optionRepository;
    private readonly IPollStatusProvider _pollStatusProvider = pollStatusProvider;

    public Task<int?> GetVotedOptionAsync(int pollId, string userId, CancellationToken cancellationToken)
    {
        return _voteRepository.GetVotedOptionAsync(pollId, userId, cancellationToken);
    }

    public async Task<VoteServiceResult> VoteAsync(int optionId, string userId, CancellationToken cancellationToken)
    {
        // Check whether option has Poll assigned to it
        int? pollId = await _optionRepository.GetOptionPollIdAsync(optionId, cancellationToken);
        if (pollId == null)
        {
            return VoteServiceResult.PollNotFound;
        }
        // Make sure that poll is active
        var pollStatus = await _pollStatusProvider.GetPollStatusAsync(pollId.Value, cancellationToken);
        if (pollStatus == null)
        {
            return VoteServiceResult.PollNotFound;
        }
        if (pollStatus.IsExpired)
        {
            return VoteServiceResult.PollExpired;
        }
        // Make sure that user isn't voting twice
        if (await _voteRepository.GetVotedOptionAsync(pollId.Value, userId, cancellationToken) != null)
        {
            return VoteServiceResult.AlreadyVoted;
        }

        // Vote
        await _voteRepository.AddVoteAsync(pollId.Value, optionId, userId, cancellationToken);

        return VoteServiceResult.Success;
    }
}
