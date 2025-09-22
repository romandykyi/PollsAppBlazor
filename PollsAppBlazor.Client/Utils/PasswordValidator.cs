using System.Text.RegularExpressions;

namespace PollsAppBlazor.Client.Utils;

public static partial class PasswordValidator
{
    public static IEnumerable<string> StrengthValidation(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            yield return "Password is required!";
            yield break;
        }
        if (password.Length < 8)
            yield return "Password must be at least of length 8";
        if (!AtLeast1CapitalLetter().IsMatch(password))
            yield return "Password must contain at least one capital letter";
        if (!AtLeast1LowercaseLetter().IsMatch(password))
            yield return "Password must contain at least one lowercase letter";
        if (!AtLeast1Digit().IsMatch(password))
            yield return "Password must contain at least one digit";
        if (password.Distinct().Count() < 3)
            yield return "Password must contain at least three unique characters";
    }

    [GeneratedRegex(@"[A-Z]")]
    private static partial Regex AtLeast1CapitalLetter();

    [GeneratedRegex(@"[a-z]")]
    private static partial Regex AtLeast1LowercaseLetter();

    [GeneratedRegex(@"[0-9]")]
    private static partial Regex AtLeast1Digit();
}
