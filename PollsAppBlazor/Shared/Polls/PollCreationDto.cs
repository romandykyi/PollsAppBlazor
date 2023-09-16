namespace PollsAppBlazor.Shared.Polls
{
    public class PollCreationDto
    {
        public const int TitleMaxSize = 60;
        public const int DescriptionMaxSize = 500;
        public const int MinOptionsCount = 2;
        public const int MaxOptionsCount = 10;

        [StringLength(TitleMaxSize, ErrorMessage = "Title cannot exceed {1} characters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
        public string Title { get; set; } = null!;
        [StringLength(DescriptionMaxSize, ErrorMessage = "Description cannot exceed {1} characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Options are required")]
        [MinLength(MinOptionsCount, ErrorMessage = "Minimal number of options is {1}")]
        [MaxLength(MaxOptionsCount, ErrorMessage = "Maximal number of options is {1}")]
        public ICollection<OptionCreationDto> Options { get; set; } = null!;
    }
}
