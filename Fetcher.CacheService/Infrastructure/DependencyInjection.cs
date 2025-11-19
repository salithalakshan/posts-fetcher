
using Fetcher.CacheService.Cache;
using Fetcher.CacheService.Data;
using Fetcher.CacheService.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Fetcher.CacheService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceRegistry(this IServiceCollection services)
    {
        services.AddSingleton<ICacheStoreService, CacheStoreService>();
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddHostedService<CacheCleanupService>();

        return services;
    }
}
