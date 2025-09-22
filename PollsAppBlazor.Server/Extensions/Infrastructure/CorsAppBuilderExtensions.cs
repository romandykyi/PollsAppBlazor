namespace PollsAppBlazor.Server.Extensions.Infrastructure;

public static class CorsAppBuilderExtensions
{
    public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
    {
        string uri = builder.Configuration["Uri:FrontendUri"] ??
            throw new InvalidOperationException("Uri:FrontendUri is misisng.");
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(p => p
                .WithOrigins(uri)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                );
        });
        return builder;
    }
}
