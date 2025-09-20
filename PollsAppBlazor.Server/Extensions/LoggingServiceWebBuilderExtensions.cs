namespace PollsAppBlazor.Server.Extensions;

public static class LoggingServiceWebBuilderExtensions
{
    public static WebApplicationBuilder AddApplicationLogging(this WebApplicationBuilder builder)
    {
        builder.Logging
            .ClearProviders()
            .AddConsole()
            .AddDebug()
            .AddAzureWebAppDiagnostics();

        return builder;
    }
}
