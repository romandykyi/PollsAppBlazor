using PollsAppBlazor.Application.Services.Interfaces;
using PollsAppBlazor.DataAccess.Aggregates;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;

namespace PollsAppBlazor.Application.Services.Implementations;

public class PollStatusProvider(IPollRepository pollRepository) : IPollStatusProvider
{
    private readonly IPollRepository _pollRepository = pollRepository;
    private readonly Dictionary<int, PollStatus?> _pollStatuses = [];

    public async Task<PollStatus?> GetPollStatusAsync(int pollId)
    {
        if (_pollStatuses.TryGetValue(pollId, out var cachedStatus))
        {
            return cachedStatus;
        }

        var status = await _pollRepository.GetPollStatusAsync(pollId);
        _pollStatuses[pollId] = status;

        return status;
    }
}
