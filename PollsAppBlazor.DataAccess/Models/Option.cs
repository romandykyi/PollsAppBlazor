using PollsAppBlazor.Shared.Options;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PollsAppBlazor.Server.DataAccess.Models
{
	public class Option
	{
		[Key]
		public int Id { get; set; }

		[StringLength(OptionCreationDto.DescriptionMaxSize)]
		public string Description { get; set; } = null!;

		[ForeignKey(nameof(Poll))]
		public int PollId { get; set; }
		/// <summary>
		/// Poll that contains this option.
		/// </summary>
		public Poll? Poll { get; set; }

		/// <summary>
		/// Votes for this option.
		/// </summary>
		public ICollection<Vote>? Votes { get; set; }
	}
}
