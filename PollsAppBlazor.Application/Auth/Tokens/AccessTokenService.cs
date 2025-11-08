using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PollsAppBlazor.Application.Extensions;
using PollsAppBlazor.Application.Options;
using PollsAppBlazor.Server.DataAccess.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PollsAppBlazor.Application.Auth.Tokens;

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

    /// <summary>
    /// Validates an access token without checking expiration time and then returns 
    /// ID of the user stored in it asynchronously.
    /// </summary>
    /// <param name="accessToken">A string that represents an access token.</param>
    /// <returns>
    /// A user ID retrieved from the access token or <see langword="null" />, if validation failed.
    /// </returns>
    public async Task<string?> GetUserIdFromExpiredTokenAsync(string accessToken)
    {
        // Validate the access token
        string key = _tokenOptions.SecretKey;
        JwtSecurityTokenHandler tokenHandler = new();
        TokenValidationParameters validationParameters = new()
        {
            ValidateLifetime = false, // Ignore expiration time
            ValidIssuer = _tokenOptions.ValidIssuer,
            ValidAudience = _tokenOptions.ValidAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
        TokenValidationResult validationResult = await tokenHandler.ValidateTokenAsync(
            accessToken, validationParameters);
        // Return null if validation failed
        if (!validationResult.IsValid) return null;

        // Get ID of the user
        JwtSecurityToken jwtToken = (JwtSecurityToken)validationResult.SecurityToken;
        System.Security.Claims.Claim? subClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
        return subClaim?.Value;
    }
}