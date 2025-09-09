namespace PollsAppBlazor.Shared.Options
{
	public class OptionViewDto
	{
		public int Id { get; set; }

		public string Description { get; set; } = null!;
		public int? VotesCount { get; set; }
	}
}
