using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.Server.Data;

namespace PollsAppBlazor.Server.Services
{
	public class OptionsService
	{
		private readonly ApplicationDbContext _dataContext;

		public OptionsService(ApplicationDbContext dataContext)
		{
			_dataContext = dataContext;
		}

        /// <summary>
        /// Get ID of a Poll that contains the Option.
        /// </summary>
        /// <param name="optionId">ID of the Option</param>
        /// <returns>
        /// ID of the Option or <see langword="null" /> if Option doesn't exist
        /// </returns>
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
