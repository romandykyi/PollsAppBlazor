namespace PollsAppBlazor.Shared.Polls
{
	public class PollEditDto
	{
		[MinLength(1, ErrorMessage = "Title cannot be empty")]
		[StringLength(PollCreationDto.TitleMaxSize, ErrorMessage = "Title cannot exceed {1} characters")]
		public string? Title { get; set; }
		[StringLength(PollCreationDto.DescriptionMaxSize, ErrorMessage = "Description cannot exceed {1} characters")]
		public string? Description { get; set; }
		public bool? ResultsVisibleBeforeVoting { get; set; }
	}
}
