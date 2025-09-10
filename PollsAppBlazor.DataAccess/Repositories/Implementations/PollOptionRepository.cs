using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;
using PollsAppBlazor.Shared.Options;

namespace PollsAppBlazor.DataAccess.Repositories.Implementations;

public class PollOptionRepository(ApplicationDbContext dbContext) : IPollOptionRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    /// <summary>
    /// Get ID of the Poll that contains the option.
    /// </summary>
    /// <param name="optionId">ID of the option</param>
    /// <returns>
    /// ID of the poll containing the option, or <see langword="null" /> if the option doesn't exist.
    /// </returns>
    public Task<int?> GetOptionPollIdAsync(int optionId)
    {
        return _dbContext.Options
            .AsNoTracking()
            .Where(o => o.Id == optionId)
            .Select(o => (int?)o.PollId)
            .FirstOrDefaultAsync();
    }

    public async Task<ICollection<OptionWithVotesViewDto>> GetPollOptionsAsync(int pollId)
    {
        var options = await _dbContext.Options
            .AsNoTracking()
            .Where(o => o.Id == pollId)
            .Select(o => new OptionWithVotesViewDto()
            {
                Id = o.Id,
                Description = o.Description,
                VotesCount = o.Votes!.Count()
            })
            .ToListAsync();

        return options;
    }
}
