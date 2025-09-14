using PollsAppBlazor.Shared.Options;

namespace PollsAppBlazor.Application.Services.Results;

public enum GetOptionsWithVotesStatus
{
    /// <summary>
    /// Operation was successfull.
    /// </summary>
    Success,
    /// <summary>
    /// Poll with the given ID was not found.
    /// </summary>
    PollNotFound,
    /// <summary>
    /// Votes are not currently visible for this poll and this user.
    /// </summary>
    NotVisible,
    /// <summary>
    /// Poll was deleted.
    /// </summary>
    PollDeleted
}

public class GetOptionsWithVotesResult(
    GetOptionsWithVotesStatus status, IEnumerable<OptionWithVotesViewDto>? options
    )
{
    public IEnumerable<OptionWithVotesViewDto>? Options { get; } = options;
    public GetOptionsWithVotesStatus Status { get; } = status;

    public bool IsSuccess => Status == GetOptionsWithVotesStatus.Success;

    public static GetOptionsWithVotesResult Success(IEnumerable<OptionWithVotesViewDto> options) =>
        new(GetOptionsWithVotesStatus.Success, options);

    public static GetOptionsWithVotesResult PollNotFound() => new(GetOptionsWithVotesStatus.PollNotFound, null);

    public static GetOptionsWithVotesResult NotVisible() => new(GetOptionsWithVotesStatus.NotVisible, null);

    public static GetOptionsWithVotesResult PollDeleted() => new(GetOptionsWithVotesStatus.PollDeleted, null);
}
