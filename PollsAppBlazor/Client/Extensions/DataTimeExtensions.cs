namespace PollsAppBlazor.Client.Extensions
{
    public static class DataTimeExtensions
    {
        public static string AsTimeAgo(this DateTimeOffset dateTime)
        {
            TimeSpan timeSpan = DateTimeOffset.Now - dateTime;

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
                        _ => $"about {timeSpan.Days / 365} years ago"
                    }
                }
            };
        }
    }
}
