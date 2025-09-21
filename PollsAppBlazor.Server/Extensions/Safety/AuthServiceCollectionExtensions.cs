using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PollsAppBlazor.Application.Auth;
using PollsAppBlazor.Server.DataAccess;
using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Server.Policy;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
        bool isProduction, IConfigurationSection identitySection, IConfigurationSection authSection)
    {
        SigningCredentials? credential = null;
        if (isProduction)
        {
            string? base64Certificate = identitySection["SigningCertificate"];
            string? base64Key = identitySection["SigningCertificateKey"];
            if (string.IsNullOrWhiteSpace(base64Certificate))
            {
                throw new InvalidOperationException("IdentityServer:SigningCertificate is not defined");
            }
            if (string.IsNullOrWhiteSpace(base64Key))
            {
                throw new InvalidOperationException("IdentityServerKey:SigningCertificateKey is not defined");
            }

            var certBytes = Convert.FromBase64String(base64Certificate);
            var keyBytes = Convert.FromBase64String(base64Key);
            var certText = Encoding.UTF8.GetString(certBytes).AsSpan();
            var keyText = Encoding.UTF8.GetString(keyBytes).AsSpan();

            var cert = X509Certificate2.CreateFromPem(certText, keyText);

            var key = new RsaSecurityKey(cert.GetRSAPrivateKey()!)
            {
                KeyId = cert.Thumbprint
            };

            credential = new(key, SecurityAlgorithms.RsaSha256);
        }

        _ = double.TryParse(authSection["RefreshTokenExpiryDays"], out double refreshTokenLifetimeDays);
        int refreshTokenLifetimeSeconds = (int)(refreshTokenLifetimeDays * 24 * 60 * 60);
        _ = double.TryParse(authSection["AccessTokenExpiryMinutes"], out double accessTokenLifetimeMinutes);
        int accessTokenLifetimeSeconds = (int)(accessTokenLifetimeMinutes * 60);
        var identityServices = services
            .AddIdentityServer(options =>
            {
                options.LicenseKey = identitySection["LicenseKey"];
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
                if (credential != null) options.SigningCredential = credential;
            });

        return services;
    }

    public static WebApplicationBuilder AddCustomizedAuth(this WebApplicationBuilder builder)
    {
        bool isProduction = builder.Environment.IsProduction();
        var identitySection = builder.Configuration.GetSection("IdentityServer");
        var authSection = builder.Configuration.GetSection("Auth");

        builder.Services
            .ConfigureIdentity()
            .ConfigureIdentityServer(isProduction, identitySection, authSection)
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
