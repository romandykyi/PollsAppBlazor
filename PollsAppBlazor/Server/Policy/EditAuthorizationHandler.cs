using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace PollsAppBlazor.Server.Policy
{
	public abstract class EditAuthorizationHandler<TRequirement> : AuthorizationHandler<TRequirement>
		where TRequirement : IAuthorizationRequirement
	{
		/// <summary>
		/// Perform a basic edit permission check.
		/// </summary>
		/// <remarks>
		/// Will fail if user is not authenticated and succeed if user can edit any item.
		/// </remarks>
		/// <param name="context"></param>
		/// <param name="requirement"></param>
		/// <returns>
		/// <see langword="true" /> if no further checks are needed;
		/// otherwise <see langword="false"/>
		/// </returns>
		protected bool BasicCheck(AuthorizationHandlerContext context, TRequirement requirement)
		{
			if (context.User.Identity == null || !context.User.IsAuthenticated())
			{
				context.Fail();
				return true;
			}
			if (Policies.UserCanEditAnything(context.User))
			{
				context.Succeed(requirement);
				return true;
			}
			return false;
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
