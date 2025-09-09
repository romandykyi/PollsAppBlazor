namespace PollsAppBlazor.Shared.Polls;

public class PollsPage
{
    /// <summary>
    /// Total number of all Polls that meet filter.
    /// </summary>
    public required int TotalPollsCount { get; set; }
    /// <summary>
    /// Polls at the current Page.
    /// </summary>
    public required IEnumerable<PollPreviewDto> Polls { get; set; }
}
