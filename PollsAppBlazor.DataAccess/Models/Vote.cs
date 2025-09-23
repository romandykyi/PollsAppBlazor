using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace PollsAppBlazor.Server.DataAccess.Models;

[Index(nameof(PollId), nameof(OptionId))]
[PrimaryKey(nameof(UserId), nameof(OptionId))]
public class Vote
{
    [ForeignKey(nameof(Poll))]
    public int PollId { get; set; }
    /// <summary>
    /// Poll that contains voted option.
    /// </summary>
    public Poll? Poll { get; set; }

    [ForeignKey(nameof(Option))]
    public int OptionId { get; set; }
    /// <summary>
    /// Option that was voten for.
    /// </summary>
    public Option? Option { get; set; }

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    /// <summary>
    /// User who voted.
    /// </summary>
    public ApplicationUser? User { get; set; }
}
