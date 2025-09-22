namespace PollsAppBlazor.Shared.Users;

public class InitiateResetPasswordDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}
