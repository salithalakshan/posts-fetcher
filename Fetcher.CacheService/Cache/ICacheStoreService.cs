
namespace Fetcher.CacheService.Cache;

public interface ICacheStoreService
{
    Task<string> GetAsync(string key, CancellationToken cancellationToken);
    Task AddAsync(string key, string value, CancellationToken cancellationToken);
    Task CleanupExpiredAsync(CancellationToken cancellationToken);
}
