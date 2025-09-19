using Microsoft.Extensions.Logging;
using PollsAppBlazor.Application.Services.Communication.Interfaces;

namespace PollsAppBlazor.Application.Services.Communication.Implementations;

public class DevEmailService(ILogger<DevEmailService> logger) : IEmailService
{
    private readonly ILogger<DevEmailService> _logger = logger;

    public Task<bool> SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email to: {To}\nSubject: {Subject}\nBody:\n{Body}", to, subject, body);

        return Task.FromResult(true);
    }
}
