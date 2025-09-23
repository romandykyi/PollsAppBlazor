using PollsAppBlazor.Shared.Options;

namespace PollsAppBlazor.DataAccess.Repositories.Interfaces;

public interface IPollOptionRepository
{
    /// <summary>
    /// Gets ID of the Poll that contains the option.
    /// </summary>
    /// <param name="optionId">ID of the option</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>
    /// ID of the poll containing the option, or <see langword="null" /> if the option doesn't exist.
    /// </returns>
    Task<int?> GetOptionPollIdAsync(int optionId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets options for the given poll, including the number of votes for each option.
    /// </summary>
    /// <param name="pollId">ID of the poll which options to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>
    /// Options of the poll with their vote counts. If the poll has no options or doesn't exist,
    /// an empty collection is returned.
    /// </returns>
    Task<ICollection<OptionWithVotesViewDto>> GetPollOptionsAsync(int pollId, CancellationToken cancellationToken);
}
