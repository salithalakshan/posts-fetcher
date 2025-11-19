using Microsoft.Data.SqlClient;

namespace Fetcher.CacheService.Data;

public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
}
