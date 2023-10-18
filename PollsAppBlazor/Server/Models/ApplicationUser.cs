using Microsoft.AspNetCore.Identity;

namespace PollsAppBlazor.Server.Models
{
	public class ApplicationUser : IdentityUser
	{
		/// <summary>
		/// Polls which were created by this user.
		/// </summary>
		public ICollection<Poll>? CreatedPolls { get; set; }
		/// <summary>
		/// Votes given by this user.
		/// </summary>
		public ICollection<Vote>? Votes { get; set; }
		/// <summary>
		/// All favorites of this user.
		/// </summary>
		public ICollection<Favorite>? Favorites { get; set; }
		/// <summary>
		/// All favorite polls of this user.
		/// </summary>
		public ICollection<Poll>? FavoritePolls { get; set; }
	}
}
