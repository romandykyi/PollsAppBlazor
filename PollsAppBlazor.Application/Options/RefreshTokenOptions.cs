namespace PollsAppBlazor.Application.Options;

/// <summary>
/// A class that contains refresh token options.
/// </summary>
public class RefreshTokenOptions
{
    /// <summary>
    /// Size of the refresh token in bytes.
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Days before token expires.
    /// </summary>
    public int ExpirationDays { get; set; }

    /// <summary>
    /// Minutes before short-lived token expires (remember me is unchecked).
    /// </summary>
    public int ShortExpirationMinutes { get; set; }

    /// <summary>
    /// A secret key used for hashing refresh tokens.
    /// </summary>
    public string SecretKey { get; set; } = null!;


    public const string SectionName = "Auth:RefreshToken";
}