namespace PollsAppBlazor.Shared.Polls
{
    public class OptionCreationDto
    {
        public const int DescriptionMaxSize = 120;

        [StringLength(DescriptionMaxSize, ErrorMessage = "Description cannot exceed {1} characters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required")]
        public string Description { get; set; } = null!;
    }
}
