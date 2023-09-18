using PollsAppBlazor.Shared.Users;
using System.Security.Claims;

namespace PollsAppBlazor.Server.Policy
{
	public static class Policies
	{
		public const string CanEditPoll = "CanEditPoll";
		public const string CanEditOption = "CanEditOption";

		/// <summary>
		/// Checks if user can edit any item(e.g. Poll, Vote) without being a creator of it.
		/// </summary>
		/// <param name="user">User that needs to be checked</param>
		/// <returns>
		/// <see langword="true" /> if user can edit something without being creator
		/// (e.g. user is Administrator or Moderator);
		/// otherwise <see langword="false"/> if user must be creator in order to edit something.
		/// </returns>
		public static bool UserCanEditAnything(ClaimsPrincipal user)
		{
			return user.IsInRole(Roles.Administrator) || user.IsInRole(Roles.Moderator);
		}
	}
}
