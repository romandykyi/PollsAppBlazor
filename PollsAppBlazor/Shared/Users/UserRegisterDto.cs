namespace PollsAppBlazor.Shared.Users;

public class UserRegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(15, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9_]*$", ErrorMessage = "Username must start with letter, be alphanumeric, underscores are allowed")]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at at max {1} characters long.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}
