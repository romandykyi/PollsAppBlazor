namespace PollsAppBlazor.Shared.Options;

public class OptionWithVotesViewDto
{
    public required int Id { get; set; }

    public required string Description { get; set; }

    public required int VotesCount { get; set; }
}
