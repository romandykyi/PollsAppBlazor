using PollsAppBlazor.Shared.Polls;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PollsAppBlazor.Server.Models
{
	public class Poll
	{
		[Key]
		public int Id { get; set; }

		[StringLength(PollCreationDto.TitleMaxSize)]
		public string Title { get; set; } = null!;
		[StringLength(PollCreationDto.DescriptionMaxSize)]
		public string? Description { get; set; }

		public DateTimeOffset CreationDate { get; set; }

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
	}
}
