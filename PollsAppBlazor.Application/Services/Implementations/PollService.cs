using PollsAppBlazor.Application.Services.Interfaces;
using PollsAppBlazor.Application.Services.Results;
using PollsAppBlazor.DataAccess.Mapping;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.DataAccess.Repositories.Options;
using PollsAppBlazor.DataAccess.Repositories.Results;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Application.Services.Implementations;

public class PollService(
    IVoteRepository voteRepository,
    IPollRepository pollRepository,
    IPollStatusProvider pollStatusProvider,
    IPollOptionRepository optionRepository,
    IFavoriteRepository favoriteRepository
    ) : IPollService
{
    private readonly IVoteRepository _voteRepository = voteRepository;
    private readonly IPollRepository _pollRepository = pollRepository;
    private readonly IPollStatusProvider _pollStatusProvider = pollStatusProvider;
    private readonly IPollOptionRepository _optionRepository = optionRepository;
    private readonly IFavoriteRepository _favoriteRepository = favoriteRepository;

    public async Task<string?> GetCreatorIdAsync(int pollId, CancellationToken cancellationToken)
    {
        return (await _pollStatusProvider.GetPollStatusAsync(pollId, cancellationToken))?.CreatorId;
    }

    public async Task<bool?> IsPollActiveAsync(int pollId, CancellationToken cancellationToken)
    {
        return !(await _pollStatusProvider.GetPollStatusAsync(pollId, cancellationToken))?.IsExpired;
    }

    public async Task<PollRetrievalResult<PollViewDto>> GetByIdAsync(int pollId, string? userId = null, CancellationToken cancellationToken = default)
    {
        static PollRetrievalResult<PollViewDto> Error(PollRetrievalError error) =>
            PollRetrievalResult<PollViewDto>.Failure(error);

        var pollStatus = await _pollStatusProvider.GetPollStatusAsync(pollId, cancellationToken);
        if (pollStatus == null) return Error(PollRetrievalError.PollNotFound);
        if (pollStatus.IsDeleted) return Error(PollRetrievalError.PollDeleted);

        PollViewDto? poll = await _pollRepository.GetByIdAsync(pollId, cancellationToken);
        if (poll == null) return Error(PollRetrievalError.PollNotFound);

        if (userId != null)
        {
            poll.IsInFavorites = await _favoriteRepository.ExistsAsync(pollId, userId, cancellationToken);
            poll.VotedOptionId = await _voteRepository.GetVotedOptionAsync(pollId, userId, cancellationToken);
        }

        return PollRetrievalResult<PollViewDto>.Success(poll);
    }

    public Task<PollsPage> GetPollsAsync(PollsPagePaginationParameters parameters, CancellationToken cancellationToken)
    {
        PollsRetrievalOptions options = new(
            Parameters: parameters,
            CreatorId: null,
            FavoritesOfUserId: null
            );
        return _pollRepository.GetPollsPageAsync(options, cancellationToken);
    }

    public async Task<GetOptionsWithVotesResult> GetOptionsWithVotesAsync(int pollId, string? userId = null, CancellationToken cancellationToken = default)
    {
        var pollStatus = await _pollStatusProvider.GetPollStatusAsync(pollId, cancellationToken);
        if (pollStatus == null) return GetOptionsWithVotesResult.PollNotFound();
        if (pollStatus.IsDeleted) return GetOptionsWithVotesResult.PollDeleted();
        if (!pollStatus.IsExpired && !pollStatus.VotesVisibleBeforeVoting)
        {
            if (string.IsNullOrEmpty(userId))
                return GetOptionsWithVotesResult.NotVisible();

            if (userId != pollStatus.CreatorId)
            {
                // Check if the user had voted
                var votedOptionId = await _voteRepository.GetVotedOptionAsync(pollId, userId, cancellationToken);
                if (votedOptionId == null)
                    return GetOptionsWithVotesResult.NotVisible();
            }
        }

        var options = await _optionRepository.GetPollOptionsAsync(pollId, cancellationToken);
        return GetOptionsWithVotesResult.Success(options);
    }

    public async Task<PollViewDto> CreatePollAsync(PollCreationDto pollDto, string creatorId, CancellationToken cancellationToken = default)
    {
        var poll = await _pollRepository.CreatePollAsync(pollDto, creatorId, cancellationToken);
        return poll.ToPollViewDto();
    }

    public async Task<PollRetrievalResult<PollCreationDto>> GetForEditById(int pollId, CancellationToken cancellationToken = default)
    {
        static PollRetrievalResult<PollCreationDto> Error(PollRetrievalError error) =>
            PollRetrievalResult<PollCreationDto>.Failure(error);

        var pollStatus = await _pollStatusProvider.GetPollStatusAsync(pollId, cancellationToken);
        if (pollStatus == null) return Error(PollRetrievalError.PollNotFound);
        if (pollStatus.IsDeleted) return Error(PollRetrievalError.PollDeleted);

        var pollDto = await _pollRepository.GetForEditByIdAsync(pollId, cancellationToken);

        return pollDto != null
            ? PollRetrievalResult<PollCreationDto>.Success(pollDto)
            : Error(PollRetrievalError.PollNotFound);
    }

    public async Task<EditPollResult> EditPollAsync(PollEditDto poll, int pollId, CancellationToken cancellationToken = default)
    {
        var pollStatus = await _pollStatusProvider.GetPollStatusAsync(pollId, cancellationToken);
        if (pollStatus == null) return EditPollResult.NotFound;
        if (pollStatus.IsDeleted) return EditPollResult.Deleted;
        if (pollStatus.IsExpired) return EditPollResult.Expired;

        return await _pollRepository.EditPollAsync(poll, pollId, cancellationToken)
            ? EditPollResult.Success
            : EditPollResult.NotFound;
    }

    public async Task<PollDeleteResult> DeletePollAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollStatus = await _pollStatusProvider.GetPollStatusAsync(pollId, cancellationToken);
        if (pollStatus == null) return PollDeleteResult.PollNotFound;
        if (pollStatus.IsDeleted) return PollDeleteResult.PollDeleted;

        return await _pollRepository.DeletePollAsync(pollId, cancellationToken)
            ? PollDeleteResult.Success
            : PollDeleteResult.PollNotFound;
    }

    public async Task<ExpirePollResult> ExpirePollAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollStatus = await _pollStatusProvider.GetPollStatusAsync(pollId, cancellationToken);
        if (pollStatus == null) return ExpirePollResult.NotFound;
        if (pollStatus.IsDeleted) return ExpirePollResult.Deleted;
        if (pollStatus.IsExpired) return ExpirePollResult.AlreadyExpired;

        return await _pollRepository.ExpirePollAsync(pollId, cancellationToken) ?
            ExpirePollResult.Success :
            ExpirePollResult.NotFound; // Should not happen
    }
}
