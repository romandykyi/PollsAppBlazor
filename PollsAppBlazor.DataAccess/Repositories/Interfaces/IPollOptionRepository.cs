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

    Task<IEnumerable<OptionViewDto>> GetPollOpti
}
