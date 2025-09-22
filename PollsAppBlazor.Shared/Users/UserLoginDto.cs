namespace PollsAppBlazor.Shared.Users;

public class UserLoginDto
{
    [Required]
    public string EmailOrUsername { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; } = false;
}
