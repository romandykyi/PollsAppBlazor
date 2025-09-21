namespace PollsAppBlazor.Application.Options;

public class UriOptions
{
    public const string SectionName = "Uri";
    public string ConfirmEmailUri { get; set; } = string.Empty;
    public string ResetPasswordUri { get; set; } = string.Empty;
}
