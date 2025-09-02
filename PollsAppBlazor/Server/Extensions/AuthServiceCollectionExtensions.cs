using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PollsAppBlazor.Server.DataAccess;
using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Server.Policy;
using System.Net;

namespace PollsAppBlazor.Server.Extensions;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddCustomizedIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 3;

                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890_";
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

        services
            .AddAuthentication()
            .AddJwtBearer();

        services.ConfigureApplicationCookie(options =>
        {
            // Return 401 when user is not authrorized
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Task.CompletedTask;
            };
            // Return 403 when user don't have access permission
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Task.CompletedTask;
            };
        });

        return services;
    }

    public static IServiceCollection AddCustomizedAuthorization(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationHandler, PollEditAuthorizationHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.CanEditPoll, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new PollEditAuthorizationRequirement());
            });
        });

        return services;
    }
}
