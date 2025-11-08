using System.Security.Claims;

namespace PollsAppBlazor.Server.Extensions.Utils;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAuthenticated(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.Identity is { IsAuthenticated: true };
    }

    /// <summary>
    /// Gets a user ID.
    /// </summary>
    /// <returns>
    /// User ID.
    /// </returns>
    public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        Claim idClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier) ??
            throw new InvalidOperationException("ID claim is missing");
        return idClaim.Value;
    }
}