namespace PollsAppBlazor.Shared.Users;

public class RefreshDto
{
    [Required]
    public string AccessToken { get; set; } = null!;
}
