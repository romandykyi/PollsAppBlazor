using PollsAppBlazor.Application.Services.Results;
using PollsAppBlazor.DataAccess.Repositories.Results;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.Application.Services.Interfaces;

/// <summary>
/// Service for managing polls.
/// </summary>
public interface IPollService
{
    /// <summary>
    /// Gets ID of user who created the poll.
    /// </summary>
    Task<string?> GetCreatorIdAsync(int pollId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks whether the poll is available for voting.
    /// </summary>
    Task<bool?> IsPollActiveAsync(int pollId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a poll by its ID.
    /// </summary>
    Task<PollRetrievalResult<PollViewDto>> GetByIdAsync(int pollId, string? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a page with polls that meet the filter conditions.
    /// </summary>
    Task<PollsPage> GetPollsAsync(PollsPagePaginationParameters parameters, CancellationToken cancellationToken);

    /// <summary>
    /// Gets options of a poll with their votes.
    /// </summary>
    Task<GetOptionsWithVotesResult> GetOptionsWithVotesAsync(int pollId, string? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a poll.
    /// </summary>
    Task<PollViewDto> CreatePollAsync(PollCreationDto pollDto, string creatorId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an editing representation of the poll by its ID.
    /// </summary>
    Task<PollRetrievalResult<PollCreationDto>> GetForEditById(int pollId, CancellationToken cancellationToken);

    /// <summary>
    /// Edits a poll. Ignores null fields in the DTO.
    /// </summary>
    Task<EditPollResult> EditPollAsync(PollEditDto poll, int pollId, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a poll by its ID.
    /// </summary>
    Task<PollDeleteResult> DeletePollAsync(int pollId, CancellationToken cancellationToken);

    /// <summary>
    /// Expires a poll by its ID.
    /// </summary>
    Task<ExpirePollResult> ExpirePollAsync(int pollId, CancellationToken cancellationToken);
}
