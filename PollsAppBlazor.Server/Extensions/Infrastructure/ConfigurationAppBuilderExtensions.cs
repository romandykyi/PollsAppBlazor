using PollsAppBlazor.Application.Options;

namespace PollsAppBlazor.Server.Extensions.Infrastructure;

public static class ConfigurationAppBuilderExtensions
{
    public static WebApplicationBuilder AddApplicationConfigurationOptions(this WebApplicationBuilder builder)
    {
        builder.Services
            .Configure<UriOptions>(
                builder.Configuration.GetSection(UriOptions.SectionName)
            )
            .Configure<RefreshTokenOptions>(
                builder.Configuration.GetSection(RefreshTokenOptions.SectionName)
            )
            .Configure<AccessTokenOptions>(
                builder.Configuration.GetSection(AccessTokenOptions.SectionName)
            );
        return builder;
    }
}
