namespace PollsAppBlazor.Shared.Users;

public class ResetPasswordDto
{
    [Required]
    public required string UserId { get; set; }
    [Required]
    public required string Token { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at at max {1} characters long.")]
    [DataType(DataType.Password)]
    public required string NewPassword { get; set; }
}
