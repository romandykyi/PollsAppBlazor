using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.Server.DataAccess.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PollsAppBlazor.DataAccess.Models;

/// <summary>
/// A class that represents the refresh token entity.
/// </summary>
[Index(nameof(TokenHash), Name = "IX_TokenValue", IsUnique = true)]
public class RefreshToken
{
    /// <summary>
    /// Maximum allowed length for the <see cref="TokenHash"/> property.
    /// </summary>
    public const int MaxTokenHashLength = 128;

    /// <summary>
    /// A primary key.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// A hashed value of the token.
    /// </summary>
    [MaxLength(MaxTokenHashLength)]
    public required string TokenHash { get; set; }
    /// <summary>
    /// ID of the user who owns this token.
    /// </summary>
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    /// <summary>
    /// Date after which the token becomes invalid.
    /// </summary>
    public required DateTime ValidTo { get; set; }
    /// <summary>
    /// Whether the token is persistent long-term.
    /// </summary>
    public required bool Persistent { get; set; }

    /// <summary>
    /// Navigation property to the user who owns this token.
    /// </summary>
    public ApplicationUser User { get; set; } = null!;
}
