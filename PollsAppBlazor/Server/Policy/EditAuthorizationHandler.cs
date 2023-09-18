using Microsoft.AspNetCore.Authorization;
using PollsAppBlazor.Shared.Users;
using System.Security.Claims;

namespace PollsAppBlazor.Server.Policy
{
    public abstract class EditAuthorizationHandler<TRequirement> : AuthorizationHandler<TRequirement>
		where TRequirement : IAuthorizationRequirement
	{
		/// <summary>
		/// Checks if user needs to be creator in order to edit something.
		/// </summary>
		/// <param name="user">User that needs to be checked</param>
		/// <returns>
		/// <see langword="true" /> if user must be creator in order to edit something;
		/// otherwise <see langword="false"/> if user can edit something without being creator
		/// (e.g. user is Administrator or Moderator).
		/// </returns>
		protected bool MustBeCreator(ClaimsPrincipal user)
		{
			return !user.IsInRole(Roles.Administrator) &&
				!user.IsInRole(Roles.Moderator);
		}

		/// <summary>
		/// Get int ID from HTTP route.
		/// </summary>
		/// <param name="context">Authorization handler context</param>
		/// <param name="idName">Name of the ID param</param>
		/// <returns>
		/// Integer ID obtained from HTTP route.
		/// </returns>
		/// <exception cref="InvalidOperationException" />
		protected int GetIntIdFromRoute(AuthorizationHandlerContext context, string idName)
		{
			if (context.Resource is not HttpContext httpContext)
			{
				throw new InvalidOperationException("Cannot get HTTP context");
			}
			var idValue = httpContext.GetRouteData().Values[idName] ??
				throw new InvalidOperationException("Cannot get poll id from HTTP context");

			return Convert.ToInt32(idValue);
		}
	}
}
