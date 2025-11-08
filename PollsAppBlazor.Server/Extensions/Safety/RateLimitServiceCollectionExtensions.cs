using Microsoft.AspNetCore.RateLimiting;
using PollsAppBlazor.Application.Services.Auth;
using PollsAppBlazor.Server.Policy;
using System.Threading.RateLimiting;

namespace PollsAppBlazor.Server.Extensions.Safety;

public static class RateLimitServiceCollectionExtensions
{
    private static RateLimiterOptions AddIPFixedWindowPolicy(this RateLimiterOptions options, string policyName, int permitLimit, TimeSpan window)
    {
        return options.AddPolicy(policyName, context =>
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
        });
    }

    private static RateLimiterOptions AddUserFixedWindowPolicy(this RateLimiterOptions options, string policyName, int permitLimit, TimeSpan window)
    {
        return options.AddPolicy(policyName, context =>
        {
            string userId = context.User.FindFirst(AppJwtClaim.UserId)?.Value ?? "unknown";
            return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
        });
    }

    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                }),
                PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5000,
                        Window = TimeSpan.FromDays(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                }));
            options.RejectionStatusCode = 429; // Too Many Requests

            options
                .AddIPFixedWindowPolicy(RateLimitingPolicy.LogInPolicy, 15, TimeSpan.FromMinutes(1))
                .AddIPFixedWindowPolicy(RateLimitingPolicy.RegisterPolicy, 15, TimeSpan.FromMinutes(1))
                .AddIPFixedWindowPolicy(RateLimitingPolicy.ResetPasswordPolicy, 5, TimeSpan.FromDays(1))
                .AddUserFixedWindowPolicy(RateLimitingPolicy.CreatePolicy, 10, TimeSpan.FromMinutes(1))
                .AddUserFixedWindowPolicy(RateLimitingPolicy.EditPolicy, 20, TimeSpan.FromMinutes(1))
                .AddUserFixedWindowPolicy(RateLimitingPolicy.VotePolicy, 2000, TimeSpan.FromDays(1));
        });
        return services;
    }
}
