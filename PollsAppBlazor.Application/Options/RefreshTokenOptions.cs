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

    public const string SectionName = "Auth:Refresh";
}