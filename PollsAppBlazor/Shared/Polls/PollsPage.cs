namespace PollsAppBlazor.Shared.Polls
{
	public class PollsPage
	{
		/// <summary>
		/// Total number of all Polls that meet filter.
		/// </summary>
		public int TotalPollsCount { get; set; }
		/// <summary>
		/// Polls at the current Page.
		/// </summary>
		public IEnumerable<PollPreviewDto> Polls { get; set; } = null!;
	}
}
