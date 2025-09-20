namespace PollsAppBlazor.Client.Extensions;

public static class DateTimeOffsetExtensions
{
    public static string AsExpiryDuration(this DateTimeOffset dateTime)
    {
        TimeSpan timeSpan = dateTime - DateTimeOffset.UtcNow;

        return (int)timeSpan.TotalHours switch
        {
            <= 1 => "in about an hour",
            < 24 => $"in {timeSpan.Hours} hours",
            _ => (int)timeSpan.TotalDays switch
            {
                <= 1 => "tomorrow",
                <= 30 => $"in {timeSpan.Days} days",

                <= 60 => "in about a month",
                < 365 => $"in {timeSpan.Days / 30} months",

                <= 365 * 2 => "in a year",
                _ => $"in {(int)(timeSpan.Days / 365.25)} years"
            }
        };
    }

    public static string AsTimeAgo(this DateTimeOffset dateTime)
    {
        TimeSpan timeSpan = DateTimeOffset.UtcNow - dateTime;

        return (int)timeSpan.TotalMinutes switch
        {
            <= 1 => "just now",
            < 60 => $"about {timeSpan.Minutes} minutes ago",
            _ => (int)timeSpan.TotalHours switch
            {
                <= 1 => "about an hour ago",
                < 24 => $"about {timeSpan.Hours} hours ago",
                _ => (int)timeSpan.TotalDays switch
                {
                    <= 1 => "yesterday",
                    <= 30 => $"about {timeSpan.Days} days ago",

                    <= 60 => "about a month ago",
                    < 365 => $"about {timeSpan.Days / 30} months ago",

                    <= 365 * 2 => "about a year ago",
                    _ => $"about {(int)(timeSpan.Days / 365.25)} years ago"
                }
            }
        };
    }
}
