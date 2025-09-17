namespace PollsAppBlazor.Application.Services.Communication.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Sends an email.
    /// </summary>
    /// <remarks>
    /// This method doesn't wait for the email to be actually sent. 
    /// It only waits for the email to be accepted by the email service provider.
    /// </remarks>
    /// <param name="to">The receiver's email.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The body of the email.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// <see langword="true" /> if the email was sent successfully; otherwise, <see langword="false" />.
    /// </returns>
    Task<bool> SendAsync(string to, string subject, string body, CancellationToken cancellationToken);
}
