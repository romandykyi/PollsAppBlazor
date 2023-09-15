namespace PollsAppBlazor.Shared
{
	public class PollViewDto
	{
		public int Id { get; set; }

		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public string Creator { get; set; } = null!;
		public DateTimeOffset CreationDate { get; set; }
		public PollStatus Status { get; set; }
		public IList<OptionViewDto> Options { get; set; } = null!;
	}
}
