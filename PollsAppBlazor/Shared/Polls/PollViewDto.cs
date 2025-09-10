using PollsAppBlazor.Shared.Options;
using PollsAppBlazor.Shared.Users;
using System.Text.Json.Serialization;

namespace PollsAppBlazor.Shared.Polls;

public class PollViewDto
{
    public required int Id { get; set; }

    public required string Title { get; set; }
    public required string? Description { get; set; }
    public required PollCreatorDto Creator { get; set; }
    public required DateTimeOffset CreationDate { get; set; }
    public required DateTimeOffset? ExpiryDate { get; set; }
    public required IList<OptionViewDto> Options { get; set; }

    public required bool ResultsVisibleBeforeVoting { get; set; }

    /// <summary>
    /// ID of the option that the current user has voted for, 
    /// or <see langword="null" /> if the user has not voted yet or
    /// if the user is not authenticated.
    /// </summary>
    public int? VotedOptionId { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the item is marked as a favorite.
    /// If his value is <see langword="null"/>, it means that
    /// the current user is not authenticated.
    /// </summary>
    public bool? IsInFavorites { get; set; }

    [JsonIgnore]
    public bool IsExpired => DateTimeOffset.Now >= ExpiryDate;
}
