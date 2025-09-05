using PollsAppBlazor.DataAccess.Repositories.Interfaces;

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
}
