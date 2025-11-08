using Microsoft.IdentityModel.Tokens;
using PollsAppBlazor.Application.Auth;
using PollsAppBlazor.Application.Options;
using PollsAppBlazor.Application.Services.Auth;
using PollsAppBlazor.Server.DataAccess.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PollsAppBlazor.Application.Extensions;

public static class ApplicationUserExtensions
{
    /// <summary>
    /// Generates a JWT for the user with the specified options.
    /// </summary>
    /// <param name="user">A user based on which the JWT is generated.</param>
    /// <param name="options">Options used for the JWT generation.</param>
    /// <param name="roles">Optional roles of the user to include.</param>
    /// <returns>A string containing a JWT for the <paramref name="user"/>.</returns>
    public static string GenerateJwt(this ApplicationUser user, AccessTokenOptions options, params IList<string> roles)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(options.SecretKey));

        List<Claim> claims =
        [
            new(AppJwtClaim.UserId, user.Id),
            new(AppJwtClaim.Email, user.Email!),
            new(AppJwtClaim.UserName, user.UserName!),
        ];
        claims.AddRange(roles.Select(x => new Claim(AppJwtClaim.Roles, x)));

        JwtSecurityToken token = new(
            issuer: options.ValidIssuer,
            audience: options.ValidAudience,
            expires: DateTime.UtcNow.AddSeconds(options.ExpirationSeconds),
            claims: claims,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return accessToken;
    }
}
