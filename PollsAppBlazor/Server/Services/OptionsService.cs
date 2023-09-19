using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.Server.Data;

namespace PollsAppBlazor.Server.Services
{
	public class OptionsService : IOptionsService
	{
		private readonly ApplicationDbContext _dataContext;

		public OptionsService(ApplicationDbContext dataContext)
		{
			_dataContext = dataContext;
		}

		/// <inheritdoc />
		public async Task<int?> GetPollIdAsync(int optionId)
		{
			return await _dataContext.Options
				.AsNoTracking()
				.Where(o => o.Id == optionId)
				.Select(o => (int?)o.PollId)
				.FirstOrDefaultAsync();
		}
	}
}
