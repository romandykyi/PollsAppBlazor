using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Shared.Options;

namespace PollsAppBlazor.Application.Services.Implementations;

public class OptionsService(IPollOptionRepository repository)
{
    private readonly IPollOptionRepository _repository = repository;

    /// <summary>
    /// Get ID of a Poll that contains the Option.
    /// </summary>
    /// <param name="optionId">ID of the Option</param>
    /// <returns>
    /// ID of the Option or <see langword="null" /> if Option doesn't exist
    /// </returns>
    public Task<int?> GetPollIdAsync(int optionId)
    {
        return _repository.GetOptionPollIdAsync(optionId);
    }

    /// <summary>
    /// Gets options of the poll with the number of votes for each option.
    /// </summary>
    /// <param name="pollId">ID of the poll which options to retrieve.</param>
    /// <returns>
    /// A collection of options with the number of votes for each option, or
    /// <see langword="null" /> if the poll doesn't exist.
    /// </returns>
    public Task<IEnumerable<OptionWithVotesViewDto>?> GetPollOptionsWithVotes(int pollId)
    {
        return _repository.GetPollOptionsAsync(pollId);
    }
}
