using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PollsAppBlazor.Application.Options;
using PollsAppBlazor.Application.Services.Auth;
using PollsAppBlazor.Application.Services.Auth.Http;
using PollsAppBlazor.Application.Services.Auth.Session;
using PollsAppBlazor.Application.Services.Auth.Tokens;
using PollsAppBlazor.Server.DataAccess;
using PollsAppBlazor.Server.DataAccess.Models;
using PollsAppBlazor.Server.Policy;
using System.Text;

namespace PollsAppBlazor.Server.Extensions.Safety;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        services
            .AddIdentityCore<ApplicationUser>(options =>
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
            .AddRoles<IdentityRole>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        return services;
    }

    public static IServiceCollection AddCustomizedAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                ServiceProvider sp = services.BuildServiceProvider();
                AccessTokenOptions accessTokenOptions = sp.GetRequiredService<IOptions<AccessTokenOptions>>().Value;

                byte[] secretKey = Encoding.UTF8.GetBytes(accessTokenOptions.SecretKey);
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromSeconds(accessTokenOptions.ClockSkewSeconds),
                    ValidIssuer = accessTokenOptions.ValidIssuer,
                    ValidAudience = accessTokenOptions.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    RoleClaimType = AppJwtClaim.Roles,
                    NameClaimType = AppJwtClaim.UserName
                };
            });
        return services
            .AddScoped<IRefreshTokenService, RefreshTokenService>()
            .AddScoped<IAccessTokenService, AccessTokenService>()
            .AddScoped<IAuthSessionManager, AuthSessionManager>()
            .AddScoped<IRefreshTokenCookieService, RefreshTokenCookeService>()
            .AddScoped<IAuthService, AuthService>();
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
