using Fetcher.CacheService.Configs;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;

namespace Fetcher.CacheService.Data;

public sealed class SqlConnectionFactory(
    IOptions<DatabaseConfig> connectionConfig
    ) : ISqlConnectionFactory
{
    private readonly string _connectionString = connectionConfig.Value.DefaultConnection;
    public SqlConnection CreateConnection() => new SqlConnection(_connectionString);
}
