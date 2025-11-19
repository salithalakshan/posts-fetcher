
namespace Fetcher.CacheService.Configs;

public class CacheSettingsConfig
{
    public int CleanupIntervalSeconds { get; init; } = 180;
    public int DefaultTtlMinutes { get; init; } = 5;
}
