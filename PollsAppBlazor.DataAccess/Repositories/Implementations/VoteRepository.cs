using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.DataAccess;
using PollsAppBlazor.Server.DataAccess.Models;

namespace PollsAppBlazor.DataAccess.Repositories.Implementations;

public class VoteRepository(ApplicationDbContext dbContext) : IVoteRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<int?> GetVotedOptionAsync(int pollId, string userId, CancellationToken cancellationToken)
    {
        return _dbContext.Votes
            .AsNoTracking()
            .Where(v => v.PollId == pollId && v.UserId == userId)
            .Select(v => (int?)v.OptionId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddVoteAsync(int pollId, int optionId, string userId, CancellationToken cancellationToken)
    {
        Vote vote = new()
        {
            PollId = pollId,
            OptionId = optionId,
            UserId = userId
        };

        _dbContext.Votes.Add(vote);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
