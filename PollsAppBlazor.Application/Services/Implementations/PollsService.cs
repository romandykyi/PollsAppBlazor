using PollsAppBlazor.Application.Services.Results;
using PollsAppBlazor.DataAccess.Mapping;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.DataAccess.Repositories.Options;
using PollsAppBlazor.DataAccess.Repositories.Results;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Application.Services.Implementations;

public class PollsService(
    IVoteRepository voteRepository,
    IPollRepository pollRepository,
    IPollOptionRepository optionRepository,
    IFavoriteRepository favoriteRepository
    )
{
    private readonly IVoteRepository _voteRepository = voteRepository;
    private readonly IPollRepository _pollRepository = pollRepository;
    private readonly IPollOptionRepository _optionRepository = optionRepository;
    private readonly IFavoriteRepository _favoriteRepository = favoriteRepository;

    /// <summary>
    /// Gets ID of user who created the poll.
    /// </summary>
    /// <param name="pollId">ID of the poll</param>
    /// <returns>
    /// ID of the user who created the poll, or <see langword="null" />
    /// if the poll was not found.
    /// </returns>
    public async Task<string?> GetCreatorIdAsync(int pollId)
    {
        return (await _pollRepository.GetPollStatusAsync(pollId))?.CreatorId;
    }

    /// <summary>
    /// Checks whether the poll is available for voting
    /// </summary>
    /// <param name="pollId">ID of the poll to check.</param>
    /// <returns>
    /// A boolean flag indicating whether poll is available for voting, or
    /// <see langword="null"/> if the poll doesn't exist.
    /// </returns>
    public async Task<bool?> IsPollActiveAsync(int pollId)
    {
        return (await _pollRepository.GetPollStatusAsync(pollId))?.IsActive;
    }

    /// <summary>
    /// Gets a poll by its ID.
    /// </summary>
    /// <param name="pollId">ID of the poll to retrieve.</param>
    /// <param name="userId">Optional ID of the user who requests a poll.</param>
    /// <returns>
    /// The result of the retrieval operation containing the poll view.
    /// </returns>
    public async Task<PollRetrievalResult<PollViewDto>> GetByIdAsync(int pollId, string? userId = null)
    {
        static PollRetrievalResult<PollViewDto> Error(PollRetrievalError error) =>
            PollRetrievalResult<PollViewDto>.Failure(error);

        var pollStatus = await _pollRepository.GetPollStatusAsync(pollId);
        if (pollStatus == null) return Error(PollRetrievalError.PollNotFound);
        if (pollStatus.IsDeleted) return Error(PollRetrievalError.PollDeleted);

        PollViewDto? poll = await _pollRepository.GetByIdAsync(pollId);
        if (poll == null) return Error(PollRetrievalError.PollNotFound);

        if (userId != null)
        {
            poll.IsInFavorites = await _favoriteRepository.ExistsAsync(pollId, userId);
            poll.VotedOptionId = await _voteRepository.GetVotedOptionAsync(pollId, userId);
        }

        return PollRetrievalResult<PollViewDto>.Success(poll);
    }

    /// <summary>
    /// Gets a page with polls that meet the filter conditions.
    /// </summary>
    /// <returns>
    /// A page with polls that match the given filter.
    /// </returns>
    public Task<PollsPage> GetPollsAsync(PollsPagePaginationParameters parameters)
    {
        PollsRetrievalOptions options = new(
            Parameters: parameters,
            CreatorId: null,
            FavoritesOfUserId: null
            );
        return _pollRepository.GetPollsPageAsync(options);
    }

    public async Task<GetOptionsWithVotesResult> GetOptionsWithVotesAsync(int pollId, string? userId = null)
    {
        var pollStatus = await _pollRepository.GetPollStatusAsync(pollId);
        if (pollStatus == null) return GetOptionsWithVotesResult.PollNotFound();
        if (!pollStatus.VotesVisibleBeforeVoting)
        {
            if (pollStatus.IsActive && userId != pollStatus.CreatorId)
                return GetOptionsWithVotesResult.NotVisible();
        }

        var options = await _optionRepository.GetPollOptionsAsync(pollId);
        return GetOptionsWithVotesResult.Success(options);
    }

    /// <summary>
    /// Creates a poll.
    /// </summary>
    /// <param name="pollDto">DTO that is used for the poll creation.</param>
    /// <param name="creatorId">ID of the user who creates the Poll.</param>
    /// <returns>A view of the created poll.</returns>
    public async Task<PollViewDto> CreatePollAsync(PollCreationDto pollDto, string creatorId)
    {
        var poll = await _pollRepository.CreatePollAsync(pollDto, creatorId);
        return poll.ToPollViewDto();
    }

    /// <summary>
    /// Gets an editing representation of the poll by its ID.
    /// </summary>
    /// <param name="pollId">ID of the poll to retrieve.</param>
    /// <returns>
    /// The result of the retrieval operation containing the poll's editing representation.
    /// </returns>
    public async Task<PollRetrievalResult<PollCreationDto>> GetForEditById(int pollId)
    {
        static PollRetrievalResult<PollCreationDto> Error(PollRetrievalError error) =>
            PollRetrievalResult<PollCreationDto>.Failure(error);

        var pollStatus = await _pollRepository.GetPollStatusAsync(pollId);
        if (pollStatus == null) return Error(PollRetrievalError.PollNotFound);
        if (pollStatus.IsDeleted) return Error(PollRetrievalError.PollDeleted);

        var pollDto = await _pollRepository.GetForEditById(pollId);

        return pollDto != null
            ? PollRetrievalResult<PollCreationDto>.Success(pollDto)
            : Error(PollRetrievalError.PollNotFound);
    }

    /// <summary>
    /// Edits a poll. Ignores null fields in the DTO.
    /// </summary>
    /// <param name="poll">DTO to use for the editing.</param>
    /// <param name="pollId">ID of the Poll to edit.</param>
    /// <returns>Operation result.</returns>
    public async Task<EditPollResult> EditPollAsync(PollEditDto poll, int pollId)
    {
        var pollStatus = await _pollRepository.GetPollStatusAsync(pollId);
        if (pollStatus == null) return EditPollResult.NotFound;
        if (pollStatus.IsDeleted) return EditPollResult.Deleted;
        if (!pollStatus.IsActive) return EditPollResult.Expired;

        return await _pollRepository.EditPollAsync(poll, pollId) != null
            ? EditPollResult.Success
            : EditPollResult.NotFound;
    }

    /// <summary>
    /// Delete a Poll by its ID.
    /// </summary>
    /// <param name="pollId">ID of a Poll that needs to be deleted</param>
    /// <returns>
    /// <see langword="true" /> if the Poll was succesfully deleted;
    /// otherwise <see langword="false"/> if the Poll was not found.
    /// </returns>
    public Task<PollDeleteResult> DeletePollAsync(int pollId)
    {
        return _pollRepository.DeletePollAsync(pollId);
    }

    /// <summary>
    /// Expires a poll by its ID.
    /// </summary>
    /// <param name="pollId">ID of the poll to expire.</param>
    /// <returns>Operation result.</returns>
    public async Task<ExpirePollResult> ExpirePollAsync(int pollId)
    {
        var pollStatus = await _pollRepository.GetPollStatusAsync(pollId);
        if (pollStatus == null) return ExpirePollResult.NotFound;
        if (pollStatus.IsDeleted) return ExpirePollResult.Deleted;
        if (!pollStatus.IsActive) return ExpirePollResult.AlreadyExpired;

        return await _pollRepository.ExpirePollAsync(pollId) ?
            ExpirePollResult.Success :
            ExpirePollResult.NotFound; // Should not happen
    }
}
