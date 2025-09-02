namespace PollsAppBlazor.Server.Extensions;

public static class DebugWebAppBuilderExtensions
{
    public static WebApplicationBuilder AddDebugOptions(this WebApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        return builder;
    }
}
