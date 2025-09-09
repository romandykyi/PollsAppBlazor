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

    public Task<IEnumerable<OptionWithVotesViewDto>?> GetPollOptionsAsync(int pollId)
    {
        // Querying the polls table ensures that we return null if the poll doesn't exist
        return _dbContext.Polls
            .AsNoTracking()
            .Where(p => p.Id == pollId)
            .Select(p => p.Options!.Select(o => new OptionWithVotesViewDto
            {
                Id = o.Id,
                Description = o.Description,
                VotesCount = o.Votes!.Count()
            }))
            .FirstOrDefaultAsync()!;
    }
}
