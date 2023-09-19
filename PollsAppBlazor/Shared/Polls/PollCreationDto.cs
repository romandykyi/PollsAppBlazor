using PollsAppBlazor.Shared.Validation;

namespace PollsAppBlazor.Shared.Polls
{
	public class PollCreationDto
	{
		public const int TitleMaxSize = 60;
		public const int DescriptionMaxSize = 500;
		public const int MinOptionsCount = 2;
		public const int MaxOptionsCount = 10;
		public const long MinExpiryDurationTicks = 10_000_000L * 60L * 60L * 1L; // 1 hour
		public const long MaxExpiryDurationTicks = 10_000_000L * 60L * 60L * 24L * 365 * 100L; // 100 years

		[StringLength(TitleMaxSize, ErrorMessage = "Title cannot exceed {1} characters")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
		public string Title { get; set; } = null!;
		[StringLength(DescriptionMaxSize, ErrorMessage = "Description cannot exceed {1} characters")]
		public string? Description { get; set; }

		[DateTimeOffsetRange(MinExpiryDurationTicks, MaxExpiryDurationTicks, 
			ErrorMessage = "The expiry date must be within a range of 1 hour to 100 years from the current time")]
		public DateTimeOffset? ExpiryDate { get; set; }

		[Required(ErrorMessage = "Options are required")]
		[MinLength(MinOptionsCount, ErrorMessage = "Minimal number of options is {1}")]
		[MaxLength(MaxOptionsCount, ErrorMessage = "Maximal number of options is {1}")]
		public ICollection<OptionCreationDto> Options { get; set; } = null!;
	}
}
