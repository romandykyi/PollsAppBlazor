namespace PollsAppBlazor.Shared.Users;

public class InitiateResetPasswordDto
{
    [Required]
    public required string EmailOrUsername { get; set; }
}
