using PollsAppBlazor.Application.Options;

namespace PollsAppBlazor.Server.Extensions.Infrastructure;

public static class ConfigurationAppBuilderExtensions
{
    public static WebApplicationBuilder AddApplicationConfigurationOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<UriOptions>(
            builder.Configuration.GetSection(UriOptions.SectionName)
        );
        return builder;
    }
}
