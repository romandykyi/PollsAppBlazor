using PollsAppBlazor.Shared.Polls;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PollsAppBlazor.Server.DataAccess.Models;

public class Poll
{
    [Key]
    public int Id { get; set; }

    [StringLength(PollCreationDto.TitleMaxSize)]
    public string Title { get; set; } = null!;
    [StringLength(PollCreationDto.DescriptionMaxSize)]
    public string? Description { get; set; }

    public bool ResultsVisibleBeforeVoting { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset? ExpiryDate { get; set; }

    /// <summary>
    /// Returns <see langword="true" /> if the poll is still active (not expired, not deleted), 
    /// otherwise <see langword="false" />. 
    /// </summary>
    [NotMapped]
    public bool IsActive => !IsDeleted && (ExpiryDate == null || DateTimeOffset.Now < ExpiryDate);

    [ForeignKey(nameof(Creator))]
    public string CreatorId { get; set; } = null!;
    /// <summary>
    /// User who created this poll.
    /// </summary>
    public ApplicationUser? Creator { get; set; }

    /// <summary>
    /// Options of this poll.
    /// </summary>
    public ICollection<Option>? Options { get; set; }

    /// <summary>
    /// All votes related to this poll.
    /// </summary>
    public ICollection<Vote>? Votes { get; set; }

    /// <summary>
    /// All favorites related to this poll.
    /// </summary>
    public ICollection<Favorite>? Favorites { get; set; }
}
