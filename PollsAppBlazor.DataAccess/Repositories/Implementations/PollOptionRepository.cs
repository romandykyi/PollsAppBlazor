using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;
using PollsAppBlazor.Shared.Options;

namespace PollsAppBlazor.DataAccess.Repositories.Implementations;

public class PollOptionRepository(ApplicationDbContext dbContext) : IPollOptionRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<int?> GetOptionPollIdAsync(int optionId, CancellationToken cancellationToken)
    {
        return _dbContext.Options
            .AsNoTracking()
            .Where(o => o.Id == optionId)
            .Select(o => (int?)o.PollId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ICollection<OptionWithVotesViewDto>> GetPollOptionsAsync(int pollId, CancellationToken cancellationToken)
    {
        var options = await _dbContext.Options
            .AsNoTracking()
            .Where(o => o.PollId == pollId)
            .Select(o => new OptionWithVotesViewDto()
            {
                Id = o.Id,
                Description = o.Description,
                VotesCount = o.Votes!.Count()
            })
            .ToListAsync(cancellationToken);

        return options;
    }
}
