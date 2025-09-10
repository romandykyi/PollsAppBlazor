using PollsAppBlazor.DataAccess.Repositories.Options;
using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.DataAccess.Repositories.Interfaces;

public interface IPollRepository
{
    /// <summary>
    /// Gets ID of the user who created the Poll.
    /// </summary>
    /// <param name="pollId">ID of the Poll.</param>
    /// <returns>
    /// ID of the user who created the poll or <see langword="null" />
    /// if Poll was not found.
    /// </returns>
    Task<string?> GetCreatorIdAsync(int pollId);

    /// <summary>
    /// Checks whether the poll is active (i.e. not expired or closed).
    /// </summary>
    /// <param name="pollId">ID of the poll to check.</param>
    /// <param name="track">
    /// Whether to track the entity in the context. 
    /// If ORM is not used, this parameter will be ignored.
    /// </param>
    /// <returns>
    /// A boolean flag indicating whether poll is available for voting, or
    /// <see langword="null"/> if the poll doesn't exist.
    /// </returns>
    Task<bool?> IsPollActiveAsync(int pollId, bool trackEntity = false);

    /// <summary>
    /// Gets the poll by its ID.
    /// </summary>
    /// <param name="pollId">ID of the Poll we need to get</param>
    /// <returns>
    /// View of the Poll, or <see langword="null" /> if Poll was not found.
    /// </returns>
    Task<PollViewDto?> GetByIdAsync(int pollId);

    /// <summary>
    /// Gets an editing representation of the poll by its ID.
    /// </summary>
    /// <param name="pollId">ID of the Poll we need to get.</param>
    /// <returns>
    /// An editing representation of the Poll, or <see langword="null" /> if 
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
    /// The updated poll or <see langword="null" /> if it does not exist.
    /// </returns>
    Task<Poll?> EditPollAsync(PollEditDto editDto, int pollId);

    /// <summary>
    /// Expires the poll.
    /// </summary>
    /// <param name="pollId">ID of the poll to expire.</param>
    /// <returns>
    /// <see langword="true" /> on success, <see langword="false"/> if poll
    /// is already expired or <see langword="null" /> if the poll was not found.
    /// </returns>
    Task<bool?> ExpirePollAsync(int pollId);

    /// <summary>
    /// Deletes a poll by its ID.
    /// </summary>
    /// <param name="pollId">ID of the poll that needs to be deleted.</param>
    /// <returns>
    /// <see langword="true" /> if the poll was succesfully deleted;
    /// otherwise <see langword="false"/> if the poll was not found.
    /// </returns>
    Task<bool> DeletePollAsync(int pollId);
}
