namespace PollsAppBlazor.Shared.Polls
{
    public class PollPreviewDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;
        public string Creator { get; set; } = null!;
        public DateTimeOffset CreationDate { get; set; }
    }
}
