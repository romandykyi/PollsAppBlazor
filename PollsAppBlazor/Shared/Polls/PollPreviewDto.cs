namespace PollsAppBlazor.Shared.Polls;

public class PollPreviewDto
{
    public required int Id { get; set; }

    public required string Title { get; set; } = null!;
    public required string Creator { get; set; } = null!;
    public required DateTimeOffset CreationDate { get; set; }
    public required DateTimeOffset? ExpiryDate { get; set; }
    public required int VotesCount { get; set; }
}
