using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;

namespace PollsAppBlazor.Server.Services;

public class VotesService(
    IVoteRepository voteRepository,
    ApplicationDbContext dataContext,
    IMemoryCache memoryCache,
    IConfiguration configuration
    )
{
    private static string VotesForOption(int optionId) => $"vts:{optionId}";

    private readonly IVoteRepository _voteRepository = voteRepository;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IConfiguration _configuration = configuration;
    private readonly ApplicationDbContext _dataContext = dataContext;

    /// <summary>
    /// Count votes for option.
    /// </summary>
    /// <param name="optionId">ID of the option which votes need to be count</param>
    /// <returns>
    /// Number of votes on the option
    /// </returns>
    public async Task<int> CountVotesAsync(int optionId)
    {
        // Get value from cache if it's available
        if (_memoryCache.TryGetValue(VotesForOption(optionId), out int cachedCount))
        {
            return cachedCount;
        }

        // Calculate the count from the database
        int count = await _dataContext.Votes
            .AsNoTracking()
            .Where(v => v.OptionId == optionId)
            .CountAsync();

        // Cache value
        var settings = _configuration.GetSection("CacheSettings");
        var duration = TimeSpan.FromMinutes(settings.GetValue<int>("VotesCountCacheDurationMinutes"));
        _memoryCache.Set(VotesForOption(optionId), count, duration);

        return count;
    }

    /// <summary>
    /// Get ID of an option which was voted by the user
    /// </summary>
    /// <param name="pollId">ID of the poll</param>
    /// <param name="userId">ID of the user</param>
    /// <returns>
    /// ID of voted by user option, or
    /// <see langword="null" /> if user didn't vote on this poll;
    /// </returns>
    public Task<int?> GetVotedOptionAsync(int pollId, string userId)
    {
        return _voteRepository.GetVotedOptionAsync(pollId, userId);
    }

    /// <summary>
    /// Vote for an option
    /// </summary>
    /// <param name="pollId">ID of the poll which contains this option</param>
    /// <param name="optionId">ID of the option</param>
    /// <param name="userId">ID of the user who votes</param>
    public Task VoteAsync(int pollId, int optionId, string userId)
    {
        return _voteRepository.VoteAsync(pollId, optionId, userId);
    }
}
