using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PollsAppBlazor.DataAccess.Aggregates;
using PollsAppBlazor.DataAccess.Extensions;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.DataAccess.Repositories.Options;
using PollsAppBlazor.Server.DataAccess;
using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Shared.Options;
using PollsAppBlazor.Shared.Polls;
using PollsAppBlazor.Shared.Users;
using System.Linq.Expressions;

namespace PollsAppBlazor.DataAccess.Repositories.Implementations;

public class PollRepository(ApplicationDbContext dbContext) : IPollRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<PollStatus?> GetPollStatusAsync(int pollId, CancellationToken cancellationToken)
    {
        return await _dbContext.Polls
            .AsNoTracking()
            .Where(p => p.Id == pollId)
            .Select(p => new PollStatus(
                p.CreatorId,
                p.ExpiryDate,
                p.ResultsVisibleBeforeVoting,
                p.IsDeleted
                )
            )
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<PollViewDto?> GetByIdAsync(int pollId, CancellationToken cancellationToken)
    {
        return _dbContext.Polls
            .Include(p => p.Creator)
            .Include(p => p.Options)
            .AsNoTracking()
            .Where(p => p.Id == pollId)
            .Select(p => new PollViewDto()
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                CreationDate = p.CreationDate,
                ExpiryDate = p.ExpiryDate,
                Creator = new PollCreatorDto()
                {
                    Id = p.CreatorId,
                    Username = p.Creator!.UserName!
                },
                ResultsVisibleBeforeVoting = p.ResultsVisibleBeforeVoting,
                // Select options
                Options = p.Options!.Select(o => new OptionViewDto()
                {
                    Id = o.Id,
                    Description = o.Description
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<PollCreationDto?> GetForEditByIdAsync(int pollId, CancellationToken cancellationToken)
    {
        return _dbContext.Polls
            .Include(p => p.Options)
            .AsNoTracking()
            .Where(p => p.Id == pollId)
            .Select(p => new PollCreationDto()
            {
                Description = p.Description,
                ExpiryDate = p.ExpiryDate,
                Options = p.Options!.Select(o => new OptionCreationDto()
                {
                    Description = o.Description
                }).ToList(),
                ResultsVisibleBeforeVoting = p.ResultsVisibleBeforeVoting,
                Title = p.Title
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PollsPage> GetPollsPageAsync(PollsRetrievalOptions options, CancellationToken cancellationToken)
    {
        var parameters = options.Parameters;

        var query = _dbContext.Polls
            .Include(p => p.Creator)
            .Include(p => p.Votes)
            .AsNoTracking();

        query = options.ApplyToQuery(query);

        // Count all matching Polls
        int count = await query.CountAsync(cancellationToken);

        // Select only needed data
        var filteredQuery = query.Select(p => new PollPreviewDto()
        {
            Id = p.Id,
            Title = p.Title,
            CreationDate = p.CreationDate,
            ExpiryDate = p.ExpiryDate,
            Creator = p.Creator == null ? "[deleted]" : (p.Creator.UserName ?? "[deleted]"),
            VotesCount = p.Votes!.Count
        });

        // Apply pagination
        filteredQuery = filteredQuery
            .Skip(parameters.PageSize * (parameters.Page - 1))
            .Take(parameters.PageSize);

        return new()
        {
            TotalPollsCount = count,
            Polls = await filteredQuery.ToListAsync(cancellationToken)
        };
    }

    public async Task<Poll> CreatePollAsync(PollCreationDto creationDto, string creatorId, CancellationToken cancellationToken)
    {
        Poll poll = new()
        {
            Title = creationDto.Title,
            CreatorId = creatorId,
            Description = creationDto.Description,
            ExpiryDate = creationDto.ExpiryDate?.ToUniversalTime(),
            ResultsVisibleBeforeVoting = creationDto.ResultsVisibleBeforeVoting,
            CreationDate = DateTimeOffset.UtcNow,
            Votes = []
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
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Load the creator before returning
        await _dbContext.Entry(poll)
            .Reference(p => p.Creator)
            .LoadAsync(cancellationToken);

        return poll;
    }

    public async Task<bool> EditPollAsync(PollEditDto editDto, int pollId, CancellationToken cancellationToken)
    {
        Expression<Func<SetPropertyCalls<Poll>, SetPropertyCalls<Poll>>>? setters = null;

        if (editDto.Title != null)
        {
            setters = setters.AppendSetProperty(s => s.SetProperty(p => p.Title, editDto.Title));
        }
        if (editDto.Description != null)
        {
            setters = setters.AppendSetProperty(s => s.SetProperty(p => p.Description, editDto.Description));
        }
        if (editDto.ResultsVisibleBeforeVoting.HasValue)
        {
            setters = setters.AppendSetProperty(s => s.SetProperty(p => p.ResultsVisibleBeforeVoting, editDto.ResultsVisibleBeforeVoting.Value));
        }

        // Avoid database call if nothing to update
        if (setters == null) return true;

        int rowsAffected = await _dbContext.Polls
            .Where(p => p.Id == pollId && !p.IsDeleted)
            .ExecuteUpdateAsync(setters, cancellationToken);

        return rowsAffected > 0;
    }

    public async Task<bool> ExpirePollAsync(int pollId, CancellationToken cancellationToken)
    {
        int rowsAffected = await _dbContext.Polls
            .Where(p => p.Id == pollId && !p.IsDeleted)
            .ExecuteUpdateAsync(
                s => s.SetProperty(p => p.ExpiryDate, DateTimeOffset.UtcNow),
                cancellationToken
                );

        return rowsAffected > 0;
    }

    public async Task<bool> DeletePollAsync(int pollId, CancellationToken cancellationToken)
    {
        int rowsAffected = await _dbContext.Polls
            .Where(p => p.Id == pollId && !p.IsDeleted)
            .ExecuteUpdateAsync(
                s => s.SetProperty(p => p.IsDeleted, true),
                cancellationToken
                );

        return rowsAffected > 0;
    }
}
