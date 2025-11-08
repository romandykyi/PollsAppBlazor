using PollsAppBlazor.Application.Admin;
using PollsAppBlazor.Application.Services.Implementations;
using PollsAppBlazor.Application.Services.Interfaces;
using PollsAppBlazor.DataAccess.Repositories.Implementations;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;

namespace PollsAppBlazor.Server.Extensions;

public static class DependencyInjectionServiceCollectionExtensions
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IFavoriteRepository, FavoriteRepository>()
            .AddScoped<IPollOptionRepository, PollOptionRepository>()
            .AddScoped<IPollRepository, PollRepository>()
            .AddScoped<IVoteRepository, VoteRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    }

    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IPollService, PollService>()
            .AddScoped<IOptionService, OptionService>()
            .AddScoped<IVoteService, VoteService>()
            .AddScoped<IFavoriteService, FavoriteService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IPollStatusProvider, PollStatusProvider>()
            .AddScoped<IAdminUserService, AdminUserService>();
    }
}
