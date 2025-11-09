using PollsAppBlazor.Server.DataAccess.Models;

namespace PollsAppBlazor.Application.Services.Auth.Tokens;

/// <summary>
/// An interface for a service that manages access tokens.
/// </summary>
public interface IAccessTokenService
{
    /// <summary>
    /// Generates an access token for the user.
    /// </summary>
    /// <param name="user">User which will receive an access token.</param>
    /// <param name="roles">Roles to inlcude in the access token.</param>
    /// <returns>
    /// A string that represents an access token.
    /// </returns>
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
}