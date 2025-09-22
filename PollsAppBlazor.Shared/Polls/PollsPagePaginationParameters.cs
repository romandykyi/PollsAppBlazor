using System.Web;

namespace PollsAppBlazor.Shared.Polls;

/// <summary>
/// A class that defines the polls page retrieval parameters.
/// </summary>
public class PollsPagePaginationParameters
{
    /// <summary>
    /// Flag which determines whether expired polls should be returned.
    /// </summary>
    /// <remarks>
    /// <see langword="false"/> by default.
    /// </remarks>
    public bool ShowExpired { get; set; } = false;
    /// <summary>
    /// Optional title of returned polls.
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// Optional username of the creator of returned polls.
    /// </summary>
    public string? Creator { get; set; }
    /// <summary>
    /// Sort mode for polls.
    /// </summary>
    public PollsSortMode SortMode { get; set; } = PollsSortMode.Newest;

    /// <summary>
    /// Page number, counting from 1.
    /// </summary>
    /// <remarks>
    /// If it is greater than total number of pages then the last page will
    /// be returned.
    /// </remarks>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be positive")]
    public int Page { get; set; } = 1;
    /// <summary>
    /// Minimal number of Polls displayed on one page(min. 5).
    /// </summary>
    [Range(5, int.MaxValue, ErrorMessage = "Page size must not be less than 5")]
    public int PageSize { get; set; } = 20;

    public string ToQueryString()
    {
        var properties = GetType().GetProperties();
        List<string> keyValues = [];

        foreach (var property in properties)
        {
            var value = property.GetValue(this);
            if (value != null)
            {
                keyValues.Add($"{property.Name}={HttpUtility.UrlEncode(value.ToString())}");
            }
        }

        return string.Join("&", keyValues);
    }
}
