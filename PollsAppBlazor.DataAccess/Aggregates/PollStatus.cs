namespace PollsAppBlazor.DataAccess.Aggregates;

public record PollStatus(
    string CreatorId,
    bool IsActive,
    bool VotesVisibleBeforeVoting,
    bool IsDeleted
    );
