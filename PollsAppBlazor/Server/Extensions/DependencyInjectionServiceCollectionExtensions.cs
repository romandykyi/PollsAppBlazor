using PollsAppBlazor.DataAccess.Repositories.Implementations;
using PollsAppBlazor.DataAccess.Repositories.Interfaces;
using PollsAppBlazor.Server.Services;

namespace PollsAppBlazor.Server.Extensions;

public static class DependencyInjectionServiceCollectionExtensions
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IFavoriteRepository, FavoriteRepository>();
    }

    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        return services
            .AddScoped<PollsService>()
            .AddScoped<OptionsService>()
            .AddScoped<VotesService>()
            .AddScoped<FavoritesService>();
    }
}
