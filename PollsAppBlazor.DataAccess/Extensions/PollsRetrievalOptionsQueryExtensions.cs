using PollsAppBlazor.DataAccess.Repositories.Options;
using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Shared.Polls;

namespace PollsAppBlazor.DataAccess.Extensions;

public static class PollsRetrievalOptionsQueryExtensions
{
    public static IQueryable<Poll> ApplyToQuery(
        this PollsRetrievalOptions options, IQueryable<Poll> query
        )
    {
        var parameters = options.Parameters;
        // Don't show deleted Polls
        if (!options.IncludeDeleted)
        {
            query = query.Where(p => !p.IsDeleted);
        }
        // Don't show expired Polls
        if (!parameters.ShowExpired)
        {
            query = query
                .Where(p => p.ExpiryDate == null || DateTimeOffset.Now < p.ExpiryDate);
        }
        // Search by title
        if (parameters.Title != null)
        {
            query = query
                .Where(p => p.Title!.Contains(parameters.Title));
        }
        // Search by creator
        if (options.CreatorId != null)
        {
            query = query.Where(p => p.CreatorId == options.CreatorId);
        }
        else if (parameters.Creator != null)
        {
            query = query
                .Where(p => p.Creator!.UserName!.Contains(parameters.Creator));
        }

        if (options.FavoritesOfUserId != null)
        {
            query = query.Where(p => p.Favorites!.Any(f => f.UserId == options.FavoritesOfUserId));
        }

        // Sort
        query = parameters.SortMode switch
        {
            PollsSortMode.MostVoted => query.OrderByDescending(p => p.Votes!.Count),
            PollsSortMode.Oldest => query.OrderBy(p => p.CreationDate),
            // Newest
            _ => query.OrderByDescending(p => p.CreationDate)
        };

        return query;
    }
}
