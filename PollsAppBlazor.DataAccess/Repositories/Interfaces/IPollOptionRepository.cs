using PollsAppBlazor.Shared.Options;

namespace PollsAppBlazor.DataAccess.Repositories.Interfaces;

public interface IPollOptionRepository
{
    /// <summary>
    /// Get ID of the Poll that contains the option.
    /// </summary>
    /// <param name="optionId">ID of the option</param>
    /// <returns>
    /// ID of the poll containing the option, or <see langword="null" /> if the option doesn't exist.
    /// </returns>
    Task<int?> GetOptionPollIdAsync(int optionId);

    /// <summary>
    /// Gets options for the given poll, including the number of votes for each option.
    /// </summary>
    /// <param name="pollId">ID of the poll which options to retrieve.</param>
    /// <returns>
    /// Options of the poll with their vote counts, or <see langword="null" /> if
    /// the poll doesn't exist.
    /// </returns>
    Task<IEnumerable<OptionWithVotesViewDto>?> GetPollOptionsAsync(int pollId);
}
