using IdentityModel;
using Microsoft.AspNetCore.RateLimiting;
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

    private static RateLimiterOptions AddIPSlidingWindowPolicy(this RateLimiterOptions options, string policyName, int permitLimit, TimeSpan window)
    {
        return options.AddPolicy(policyName, context =>
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return RateLimitPartition.GetSlidingWindowLimiter(clientIp, _ => new SlidingWindowRateLimiterOptions
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
            string userId = context.User.FindFirst(JwtClaimTypes.Subject)?.Value ?? "unknown";
            return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
        });
    }

    private static RateLimiterOptions AddUserSlidingWindowPolicy(this RateLimiterOptions options, string policyName, int permitLimit, TimeSpan window)
    {
        return options.AddPolicy(policyName, context =>
        {
            string userId = context.User.FindFirst(JwtClaimTypes.Subject)?.Value ?? "unknown";
            return RateLimitPartition.GetSlidingWindowLimiter(userId, _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
        });
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });
            options.RejectionStatusCode = 429; // Too Many Requests

            options
                .AddIPFixedWindowPolicy(RateLimitingPolicy.LogInShortPolicy, 15, TimeSpan.FromMinutes(1))
                .AddIPSlidingWindowPolicy(RateLimitingPolicy.LogInLongPolicy, 100, TimeSpan.FromDays(1))
                .AddIPFixedWindowPolicy(RateLimitingPolicy.RegisterShortPolicy, 15, TimeSpan.FromMinutes(1))
                .AddIPSlidingWindowPolicy(RateLimitingPolicy.RegisterLongPolicy, 100, TimeSpan.FromDays(1))
                .AddUserSlidingWindowPolicy(RateLimitingPolicy.CreateShortPolicy, 10, TimeSpan.FromMinutes(1))
                .AddUserSlidingWindowPolicy(RateLimitingPolicy.CreateLongPolicy, 25, TimeSpan.FromDays(1))
                .AddUserFixedWindowPolicy(RateLimitingPolicy.EditShortPolicy, 15, TimeSpan.FromMinutes(1))
                .AddUserSlidingWindowPolicy(RateLimitingPolicy.EditLongPolicy, 500, TimeSpan.FromDays(1))
                .AddUserSlidingWindowPolicy(RateLimitingPolicy.VotePolicy, 1000, TimeSpan.FromDays(1));
        });
        return services;
    }
}
