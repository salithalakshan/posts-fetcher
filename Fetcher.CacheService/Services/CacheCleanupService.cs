using Fetcher.CacheService.Cache;
using Fetcher.CacheService.Configs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fetcher.CacheService.Services
{
    public sealed class CacheCleanupService(
        ICacheStoreService cacheStoreService,
        ILogger<CacheCleanupService> logger,
        IOptions<CacheSettingsConfig> cacheConfig  
        ) : BackgroundService
    {
        private readonly ICacheStoreService _cacheStoreService = cacheStoreService;
        private readonly ILogger<CacheCleanupService> _logger = logger;
        private readonly CacheSettingsConfig _options = cacheConfig.Value;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await _cacheStoreService.CleanupExpiredAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while cleaning up cache entries");
                }

                try
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(_options.CleanupIntervalSeconds),
                        cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    
                }
            }
        }

    }
}
