namespace PollsAppBlazor.Application.Services.Results;

public enum VoteServiceResult
{
    /// <summary>
    /// Successfully voted.
    /// </summary>
    Success,
    /// <summary>
    /// Poll was expired.
    /// </summary>
    PollExpired,
    /// <summary>
    /// Poll was not found.
    /// </summary>
    PollNotFound,
    /// <summary>
    /// Attempt to voted twice.
    /// </summary>
    AlreadyVoted,
}
