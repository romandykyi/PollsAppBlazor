using PollsAppBlazor.Shared.Options;
using System.Text.Json.Serialization;

namespace PollsAppBlazor.Shared.Polls;

public class PollViewDto
{
    public required int Id { get; set; }

    public required string Title { get; set; }
    public required string? Description { get; set; }
    public required string Creator { get; set; }
    public required DateTimeOffset CreationDate { get; set; }
    public required DateTimeOffset? ExpiryDate { get; set; }
    public required IList<OptionViewDto> Options { get; set; }

    public required bool ResultsVisibleBeforeVoting { get; set; }

    public required int? VotedOptionId { get; set; }

    [JsonIgnore]
    public bool IsExpired => DateTimeOffset.Now >= ExpiryDate;
    [JsonIgnore]
    public string CreatorId { get; set; } = null!;
}
