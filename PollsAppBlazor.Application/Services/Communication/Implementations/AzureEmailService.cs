using Azure;
using Azure.Communication.Email;
using PollsAppBlazor.Application.Services.Communication.Interfaces;

namespace PollsAppBlazor.Application.Services.Communication.Implementations;

public class AzureEmailService(
    string senderAddress,
    EmailClient emailClient) : IEmailService
{
    private readonly string _senderAddress = senderAddress;
    private readonly EmailClient _emailClient = emailClient;

    public async Task<bool> SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        await _emailClient.SendAsync(
            WaitUntil.Started,
            _senderAddress,
            to,
            subject,
            body,
            null,
            cancellationToken
            );

        return true;
    }
}
