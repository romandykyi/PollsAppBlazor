using PollsAppBlazor.Application.Services.Interfaces;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;

namespace PollsAppBlazor.Application.Services.Implementations;

public class OptionService(IPollOptionRepository repository) : IOptionService
{
    private readonly IPollOptionRepository _repository = repository;

    public Task<int?> GetPollIdAsync(int optionId, CancellationToken cancellationToken)
    {
        return _repository.GetOptionPollIdAsync(optionId, cancellationToken);
    }
}
