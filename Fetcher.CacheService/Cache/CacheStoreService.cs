
using Fetcher.CacheService.Configs;
using Fetcher.CacheService.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fetcher.CacheService.Cache;

public sealed class CacheStoreService(
    ISqlConnectionFactory sqlConnectionFactory,
    ILogger<CacheStoreService> logger,
    IOptions<CacheSettingsConfig> cacheSettingsConfig
    ) : ICacheStoreService
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
    private readonly ILogger<CacheStoreService> _logger = logger;
    private readonly CacheSettingsConfig _cacheSettingsConfig = cacheSettingsConfig.Value;
    public async Task<string> GetAsync(string key, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT JsonValue
            FROM dbo.CacheEntry
            WHERE CacheKey = @Key AND ExpiresAtUtc > GETUTCDATE()
            """;

        await using var connection = _sqlConnectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Key", key);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if(!await reader.ReadAsync(cancellationToken))
        {
            _logger.LogInformation("Cache miss for key: {Key}", key);
            return default;
        }

        var value = reader.GetString(0);
        return value;
    }
    public async Task AddAsync(string key, string value, CancellationToken cancellationToken)
    {
        const string sql = """
                       INSERT INTO dbo.CacheEntry (CacheKey, JsonValue, CreatedAtUtc, ExpiresAtUtc)
                       VALUES (@Key, @JsonValue, @CreatedAtUtc, @ExpiresAtUtc);
                       """;
        var createdAtUtc = DateTime.UtcNow;
        var expiresAtUtc = createdAtUtc.Add(TimeSpan.FromMinutes(_cacheSettingsConfig.DefaultTtlMinutes));

        await using var connection = _sqlConnectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Key", key);
        command.Parameters.AddWithValue("@JsonValue", value);
        command.Parameters.AddWithValue("@CreatedAtUtc", createdAtUtc);
        command.Parameters.AddWithValue("@ExpiresAtUtc", expiresAtUtc);

        await command.ExecuteNonQueryAsync(cancellationToken);

        logger.LogInformation("Cache entry set for key {Key}", key);

    }

    public async Task CleanupExpiredAsync(CancellationToken cancellationToken)
    {
        const string sql = """
                           DELETE FROM dbo.CacheEntry
                           WHERE ExpiresAtUtc <= SYSUTCDATETIME();
                           """;

        await using var connection = _sqlConnectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        var affected = await command.ExecuteNonQueryAsync(cancellationToken);

        if (affected > 0)
        {
            _logger.LogInformation("Deleted {Count} expired cache entries", affected);
        }
    }

}
