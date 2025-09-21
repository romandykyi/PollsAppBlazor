namespace PollsAppBlazor.Application.Emails;

public static class AuthEmails
{
    public const string ConfirmationEmailBody = @"
<p>Hi {0},</p>

<p>Thank you for registering with <strong>PollsAppBlazor</strong>! Before you can start creating and participating in polls, we need to verify your email address.</p>

<p>Please confirm your email by clicking the link below:</p>

<p><a href='{1}' style='background-color:#4CAF50; color:white; padding:10px 20px; text-decoration:none; border-radius:5px;'>Confirm My Email</a></p>

<p>If you did not register for Polls App Blazor, please ignore this email.</p>

<p>Welcome aboard,<br>
<strong>Roman Dykyi</strong>
</p>";

    public const string ResetPasswordEmailBody = @"
<p>Hi {0},</p>

<p>We received a request to reset your password for your <strong>PollsAppBlazor</strong> account.</p>

<p>You can reset your password by clicking the link below:</p>

<p><a href='{1}' style='background-color:#f44336; color:white; padding:10px 20px; text-decoration:none; border-radius:5px;'>Reset My Password</a></p>

<p>If you did not request a password reset, please ignore this email. Your account will remain secure.</p>

<p>Thank you,<br>
<strong>Roman Dykyi</strong>
</p>
";
}
