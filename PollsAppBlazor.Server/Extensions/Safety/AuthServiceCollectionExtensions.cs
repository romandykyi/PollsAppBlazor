using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PollsAppBlazor.Application.Auth;
using PollsAppBlazor.Server.DataAccess;
using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Server.Policy;
using System.Net;
using DuendeClient = Duende.IdentityServer.Models.Client;

namespace PollsAppBlazor.Server.Extensions.Safety;

public static class AuthServiceCollectionExtensions
{
    private static IServiceCollection ConfigureIdentity(this IServiceCollection services)
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

                options.SignIn.RequireConfirmedEmail = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890_";
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }

    private static IServiceCollection ConfigureCookie(this IServiceCollection services)
    {
        return services.ConfigureApplicationCookie(options =>
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
    }

    private static IServiceCollection ConfigureIdentityServer(this IServiceCollection services,
        string? licenseKey, IConfigurationSection authSection)
    {
        _ = double.TryParse(authSection["RefreshTokenExpiryDays"], out double refreshTokenLifetimeDays);
        int refreshTokenLifetimeSeconds = (int)(refreshTokenLifetimeDays * 24 * 60 * 60);
        _ = double.TryParse(authSection["AccessTokenExpiryMinutes"], out double accessTokenLifetimeMinutes);
        int accessTokenLifetimeSeconds = (int)(accessTokenLifetimeMinutes * 60);

        services
            .AddIdentityServer(options =>
            {
                options.LicenseKey = licenseKey;
            })
            .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
            {
                options.Clients.Add(new DuendeClient()
                {
                    ClientId = "BlazorWasmClient",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowedScopes = { "openid", "profile", "api1", "offline_access" },
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    AccessTokenLifetime = accessTokenLifetimeSeconds,
                    SlidingRefreshTokenLifetime = refreshTokenLifetimeSeconds
                });
            })
            .AddDeveloperSigningCredential();

        return services;
    }

    public static WebApplicationBuilder AddCustomizedAuth(this WebApplicationBuilder builder)
    {
        string? licenseKey = builder.Configuration["IdentityServer:LicenseKey"];
        var authSection = builder.Configuration.GetSection("Auth");

        builder.Services
            .ConfigureIdentity()
            .ConfigureIdentityServer(licenseKey, authSection)
            .ConfigureCookie()
            .AddTransient<IProfileService, ApplicationProfileService>();

        return builder;
    }

    public static IServiceCollection AddCustomizedAuthorization(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationHandler, PollEditAuthorizationHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.CanEditPoll, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new PollEditAuthorizationRequirement());
            });

        return services;
    }
}
