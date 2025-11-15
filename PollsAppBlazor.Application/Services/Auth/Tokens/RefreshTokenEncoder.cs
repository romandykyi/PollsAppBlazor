namespace PollsAppBlazor.Application.Services.Auth.Tokens;

public static class RefreshTokenEncoder
{
    /// <summary>
    /// Encodes the specified refresh token components into a single string.
    /// </summary>
    /// <param name="tokenId">The token ID.</param>
    /// <param name="tokenHash"></param>
    /// <returns>
    /// An encoded refresh token string in "{id}.{hash}" format.
    /// </returns>
    public static string Encode(Guid tokenId, string tokenHash)
    {
        string convertedId = tokenId.ToString("N");
        return $"{convertedId}.{tokenHash}";
    }

    /// <summary>
    /// Tries to decode the specified refresh token into its components.
    /// </summary>
    /// <param name="token">The token to decode.</param>
    /// <param name="tokenId">The parsed token ID.</param>
    /// <param name="tokenHash">The parsed tokan hash.</param>
    /// <returns>
    /// <see langword="true" /> the token was successfully decoded; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryDecode(string token, out Guid tokenId, out string tokenHash)
    {
        tokenId = Guid.Empty;
        tokenHash = string.Empty;

        string[] tokenParts = token.Split('.');
        if (tokenParts.Length != 2)
        {
            return false;
        }

        if (!Guid.TryParseExact(tokenParts[0], "N", out tokenId))
        {
            return false;
        }
        tokenHash = tokenParts[1];

        return true;
    }
}
