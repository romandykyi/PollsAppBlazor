using Microsoft.Extensions.Options;
using PollsAppBlazor.Application.Extensions;
using PollsAppBlazor.Application.Options;
using PollsAppBlazor.Server.DataAccess.Models;

namespace PollsAppBlazor.Application.Services.Auth.Tokens;

/// <summary>
/// A service that manages access tokens.
/// </summary>
public class AccessTokenService(IOptions<AccessTokenOptions> options) : IAccessTokenService
{
    private readonly AccessTokenOptions _tokenOptions = options.Value;

    /// <summary>
    /// Generates an access token for the user.
    /// </summary>
    /// <param name="user">User which will receive an access token.</param>
    /// <param name="roles">Roles to inlcude in the access token.</param>
    /// <returns>
    /// A string that represents an access token.
    /// </returns>
    public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        return user.GenerateJwt(_tokenOptions, roles);
    }
}