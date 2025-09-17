using Azure.Communication.Email;
using PollsAppBlazor.Application.Services.Communication.Implementations;
using PollsAppBlazor.Application.Services.Communication.Interfaces;

namespace PollsAppBlazor.Server.Extensions;

public static class CommunicationAppBuilderExtensions
{
    public static WebApplicationBuilder ConfigureEmailService(this WebApplicationBuilder builder)
    {
        var senderAddress = builder.Configuration["Communication:Email:SenderAddress"];
        var connectionString = builder.Configuration["Communication:Email:ConnectionString"];
        if (string.IsNullOrWhiteSpace(senderAddress))
        {
            throw new InvalidOperationException("Email sender address is not configured.");
        }
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Email connection string is not configured.");
        }

        EmailClient emailClient = new(connectionString);
        AzureEmailService emailService = new(senderAddress, emailClient);

        builder.Services.AddSingleton<IEmailService>(emailService);

        return builder;
    }
}
