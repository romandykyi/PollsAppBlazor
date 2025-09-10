using PollsAppBlazor.DataAccess.Repositories.Interfaces;

namespace PollsAppBlazor.Application.Services.Implementations;

public class OptionsService(IPollOptionRepository repository)
{
    private readonly IPollOptionRepository _repository = repository;

    /// <summary>
    /// Gets ID of the Poll that contains the option.
    /// </summary>
    /// <param name="optionId">ID of the option.</param>
    /// <returns>
    /// ID of the option or <see langword="null" /> if it doesn't exist.
    /// </returns>
    public Task<int?> GetPollIdAsync(int optionId)
    {
        return _repository.GetOptionPollIdAsync(optionId);
    }
}
