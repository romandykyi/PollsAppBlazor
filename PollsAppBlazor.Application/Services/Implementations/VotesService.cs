using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using PollsAppBlazor.Application.Services.Results;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;

namespace PollsAppBlazor.Application.Services.Implementations;

public class VotesService(
    IVoteRepository voteRepository,
    IPollOptionRepository optionRepository,
    IPollRepository pollRepository,
    ApplicationDbContext dataContext,
    IMemoryCache memoryCache,
    IConfiguration configuration
    )
{
    private static string VotesForOption(int optionId) => $"vts:{optionId}";

    private readonly IVoteRepository _voteRepository = voteRepository;
    private readonly IPollOptionRepository _optionRepository = optionRepository;
    private readonly IPollRepository _pollsService = pollRepository;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IConfiguration _configuration = configuration;
    private readonly ApplicationDbContext _dataContext = dataContext;

    /// <summary>
    /// Counts votes for the option.
    /// </summary>
    /// <param name="optionId">ID of the option which votes to count.</param>
    /// <returns>
    /// Number of votes for the option.
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
    /// Gets ID of an option which was voted by the user
    /// </summary>
    /// <param name="pollId">ID of the poll.</param>
    /// <param name="userId">ID of the user.</param>
    /// <returns>
    /// ID of the voted option, or <see langword="null" /> if user didn't vote on this poll.
    /// </returns>
    public Task<int?> GetVotedOptionAsync(int pollId, string userId)
    {
        return _voteRepository.GetVotedOptionAsync(pollId, userId);
    }

    /// <summary>
    /// Adds a user vote for the option
    /// </summary>
    /// <param name="optionId">ID of the option.</param>
    /// <param name="userId">ID of the voter.</param>
    public async Task<VoteServiceResult> VoteAsync(int optionId, string userId)
    {
        // Check whether option has Poll assigned to it
        int? pollId = await _optionRepository.GetOptionPollIdAsync(optionId);
        if (pollId == null)
        {
            return VoteServiceResult.PollNotFound;
        }
        // Make sure that poll is active
        if (await _pollsService.IsPollActiveAsync(pollId.Value) != true)
        {
            return VoteServiceResult.PollExpired;
        }
        // Make sure that user isn't voting twice
        if (await _voteRepository.GetVotedOptionAsync(pollId.Value, userId) != null)
        {
            return VoteServiceResult.AlreadyVoted;
        }

        // Vote
        await _voteRepository.AddVoteAsync(pollId.Value, optionId, userId);

        return VoteServiceResult.Success; ;
    }
}
