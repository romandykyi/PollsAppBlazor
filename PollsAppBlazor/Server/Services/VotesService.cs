using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PollsAppBlazor.Server.Data;
using PollsAppBlazor.Server.Models;

namespace PollsAppBlazor.Server.Services
{
	public class VotesService : IVotesService
	{
		private static string VotesForOption(int optionId) => $"vts:{optionId}";

		private readonly IMemoryCache _memoryCache;
		private readonly IConfiguration _configuration;
		private readonly ApplicationDbContext _dataContext;

		public VotesService(ApplicationDbContext dataContext, IMemoryCache memoryCache, IConfiguration configuration)
		{
			_dataContext = dataContext;
			_memoryCache = memoryCache;
			_configuration = configuration;
		}

		/// <inheritdoc />
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

		/// <inheritdoc />
		public async Task<int?> GetVotedOptionAsync(int pollId, string userId)
		{
			return await _dataContext.Votes
				.AsNoTracking()
				.Where(v => v.PollId == pollId && v.UserId == userId)
				.Select(v => (int?)v.OptionId)
				.FirstOrDefaultAsync();
		}

		/// <inheritdoc />
		public async Task VoteAsync(int pollId, int optionId, string userId)
		{
			Vote vote = new()
			{
				PollId = pollId,
				OptionId = optionId,
				UserId = userId
			};

			_dataContext.Votes.Add(vote);
			await _dataContext.SaveChangesAsync();

			// Invalidate the cache
			_memoryCache.Remove(VotesForOption(optionId));
		}
	}
}
