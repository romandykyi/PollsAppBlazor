namespace PollsAppBlazor.DataAccess.Aggregates;

public record PollStatus(
    string CreatorId,
    DateTimeOffset? ExpiryDate,
    bool VotesVisibleBeforeVoting,
    bool IsDeleted
    )
{
    public bool IsExpired => ExpiryDate != null && ExpiryDate < DateTimeOffset.UtcNow;
}