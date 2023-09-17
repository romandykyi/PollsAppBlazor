namespace PollsAppBlazor.Shared.Polls
{
	public class PollViewDto
	{
		public int Id { get; set; }

		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public string Creator { get; set; } = null!;
		public DateTimeOffset CreationDate { get; set; }
		public IList<OptionViewDto> Options { get; set; } = null!;

		public bool IsOwnedByCurrentUser { get; set; }
		public bool AreVotesVisible { get; set; }
		public int? VotedOptionId { get; set; }
	}
}
