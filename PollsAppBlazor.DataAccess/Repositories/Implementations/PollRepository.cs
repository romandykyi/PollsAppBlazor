using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.DataAccess.Aggregates;
using PollsAppBlazor.DataAccess.Extensions;
using PollsAppBlazor.DataAccess.Mapping;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.DataAccess.Repositories.Options;
using PollsAppBlazor.Server.DataAccess;
using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.DataAccess.Repositories.Implementations;

public class PollRepository(ApplicationDbContext dbContext) : IPollRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<PollStatus?> GetPollStatusAsync(int pollId, bool trackEntity = false)
    {
        if (trackEntity)
        {
            var poll = await _dbContext.Polls.FindAsync(pollId);
            return poll?.ToPollStatus();
        }
        return await _dbContext.Polls
            .AsNoTracking()
            .Where(p => p.Id == pollId)
            .Select(p => p.ToPollStatus())
            .FirstOrDefaultAsync();
    }

    public Task<PollViewDto?> GetByIdAsync(int pollId)
    {
        return _dbContext.Polls
            .AsNoTracking()
            .Where(p => p.Id == pollId)
            .Select(p => p.ToPollViewDto())
            .FirstOrDefaultAsync();
    }

    public Task<PollCreationDto?> GetForEditById(int pollId)
    {
        return _dbContext.Polls
            .AsNoTracking()
            .Where(p => p.Id == pollId)
            .Select(p => p.ToPollCreationDto())
            .FirstOrDefaultAsync();
    }

    public async Task<PollsPage> GetPollsPageAsync(PollsRetrievalOptions options)
    {
        var parameters = options.Parameters;

        var query = _dbContext.Polls
            .Include(p => p.Creator)
            .Include(p => p.Votes)
            .AsNoTracking();

        query = options.ApplyToQuery(query);

        // Count all matching Polls
        int count = await query.CountAsync();

        // Select only needed data
        var filteredQuery = query.Select(p => new PollPreviewDto()
        {
            Id = p.Id,
            Title = p.Title,
            CreationDate = p.CreationDate,
            ExpiryDate = p.ExpiryDate,
            Creator = p.Creator!.UserName!,
            VotesCount = p.Votes!.Count
        });

        // Apply pagination
        filteredQuery = filteredQuery
            .Skip(parameters.PageSize * (parameters.Page - 1))
            .Take(parameters.PageSize);

        return new()
        {
            TotalPollsCount = count,
            Polls = await filteredQuery.ToListAsync()
        };
    }

    public async Task<Poll> CreatePollAsync(PollCreationDto creationDto, string creatorId)
    {
        Poll poll = new()
        {
            Title = creationDto.Title,
            CreatorId = creatorId,
            Description = creationDto.Description,
            ExpiryDate = creationDto.ExpiryDate,
            ResultsVisibleBeforeVoting = creationDto.ResultsVisibleBeforeVoting,
            CreationDate = DateTimeOffset.Now
        };
        List<Option> options = creationDto.Options
            .Select(o => new Option()
            {
                Poll = poll,
                Description = o.Description
            })
            .ToList();

        poll.Options = options;

        _dbContext.Polls.Add(poll);
        _dbContext.Options.AddRange(options);
        await _dbContext.SaveChangesAsync();

        return poll;
    }

    public async Task<Poll?> EditPollAsync(PollEditDto editDto, int pollId)
    {
        Poll? poll = await _dbContext.Polls.FindAsync(pollId);
        if (poll == null) return null;

        if (editDto.Title != null)
        {
            poll.Title = editDto.Title;
        }
        if (editDto.Description != null)
        {
            poll.Description = editDto.Description;
        }
        if (editDto.ResultsVisibleBeforeVoting.HasValue)
        {
            poll.ResultsVisibleBeforeVoting = editDto.ResultsVisibleBeforeVoting.Value;
        }

        await _dbContext.SaveChangesAsync();

        return poll;
    }

    public async Task<bool?> ExpirePollAsync(int pollId)
    {
        Poll? poll = await _dbContext.Polls.FindAsync(pollId);
        if (poll == null) return null;

        if (!poll.IsActive) return false;

        poll.ExpiryDate = DateTimeOffset.Now;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeletePollAsync(int pollId)
    {
        int entriesRemoved = await _dbContext.Polls
            .Where(p => p.Id == pollId)
            .ExecuteDeleteAsync();

        return entriesRemoved > 0;
    }
}
