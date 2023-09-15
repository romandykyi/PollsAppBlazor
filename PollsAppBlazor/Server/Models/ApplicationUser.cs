using Microsoft.AspNetCore.Identity;

namespace PollsAppBlazor.Server.Models
{
	public class ApplicationUser : IdentityUser
	{
		/// <summary>
		/// Polls which were createn by this user.
		/// </summary>
		public ICollection<Poll>? CreatedPolls { get; set; }
		/// <summary>
		/// Votes given by this user.
		/// </summary>
		public ICollection<Vote>? Votes { get; set; }
	}
}
