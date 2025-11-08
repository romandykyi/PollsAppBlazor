using System.IdentityModel.Tokens.Jwt;

namespace PollsAppBlazor.Application.Services.Auth;

public static class AppJwtClaim
{
    public const string UserId = JwtRegisteredClaimNames.Sub;
    public const string Email = JwtRegisteredClaimNames.Email;
    public const string UserName = JwtRegisteredClaimNames.UniqueName;
    public const string Roles = "role";
}
