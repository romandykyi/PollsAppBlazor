using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.Server.Data;
using PollsAppBlazor.Server.Models;
using PollsAppBlazor.Shared.Polls;

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

		/// <inheritdoc />
		public async Task<bool> EditOptionAsync(OptionEditDto option, int optionId)
		{
			Option? actualOption = await _dataContext.Options
				.FirstOrDefaultAsync(o => o.Id == optionId);
			if (actualOption == null)
			{
				return false;
			}

			actualOption.Description = option.Description;

			_dataContext.Update(actualOption);
			await _dataContext.SaveChangesAsync();

			return true;
		}
	}
}
