using PollsAppBlazor.DataAccess.Aggregates;
using PollsAppBlazor.DataAccess.Repositories.Options;
using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.DataAccess.Repositories.Interfaces;

public interface IPollRepository
{
    /// <summary>
    /// Gets the poll's status by its ID.
    /// </summary>
    /// <param name="pollId">ID of the poll.</param>
    /// <returns>
    /// The poll's status or <see langword="null" /> if it was not found.
    /// </returns>
    Task<PollStatus?> GetPollStatusAsync(int pollId);

    /// <summary>
    /// Gets the poll by its ID.
    /// </summary>
    /// <param name="pollId">ID of the poll we need to get</param>
    /// <returns>
    /// View of the poll, or <see langword="null" /> if poll was not found.
    /// </returns>
    Task<PollViewDto?> GetByIdAsync(int pollId);

    /// <summary>
    /// Gets an editing representation of the poll by its ID.
    /// </summary>
    /// <param name="pollId">ID of the poll we need to get.</param>
    /// <returns>
    /// An editing representation of the poll, or <see langword="null" /> if 
    /// the poll was not found.
    /// </returns>
    Task<PollCreationDto?> GetForEditById(int pollId);

    /// <summary>
    /// Gets the polls page using the given options.
    /// </summary>
    /// <param name="options">Options to use to retrieve the page.</param>
    /// <returns>A page containing the requested polls.</returns>
    Task<PollsPage> GetPollsPageAsync(PollsRetrievalOptions options);

    /// <summary>
    /// Creates a poll.
    /// </summary>
    /// <param name="creationDto">Poll DTO used for its creation.</param>
    /// <param name="creatorId">ID of the user who creates this poll.</param>
    /// <returns>A created poll.</returns>
    Task<Poll> CreatePollAsync(PollCreationDto creationDto, string creatorId);

    /// <summary>
    /// Updates the poll, ignoring the <see langword="null" /> properties.
    /// </summary>
    /// <param name="editDto">The poll DTO to use.</param>
    /// <param name="pollId">ID of the poll to update.</param>
    /// <returns>
    /// <see cref="true" /> on success, <see langword="false"/> if poll
    /// was not found or deleted.
    /// </returns>
    Task<bool> EditPollAsync(PollEditDto editDto, int pollId);

    /// <summary>
    /// Expires the poll.
    /// </summary>
    /// <param name="pollId">ID of the poll to expire.</param>
    /// <returns>
    /// <see langword="true" /> on success, <see langword="false"/> if poll
    /// was not found or deleted.
    /// </returns>
    Task<bool> ExpirePollAsync(int pollId);

    /// <summary>
    /// Deletes a poll by its ID.
    /// </summary>
    /// <param name="pollId">ID of the poll that needs to be deleted.</param>
    /// <returns>
    /// <see langword="true" /> on success, <see langword="false"/> if poll
    /// was not found or deleted.
    /// </returns>
    Task<bool> DeletePollAsync(int pollId);
}
