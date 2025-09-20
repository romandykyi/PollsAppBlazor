using Microsoft.EntityFrameworkCore;
using PollsAppBlazor.Server.DataAccess;

namespace PollsAppBlazor.Server.Extensions.Infrastructure;

public static class DataWebAppBuilderExtensions
{
    public static WebApplicationBuilder ConnectDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString)
            );

        return builder;
    }
}
