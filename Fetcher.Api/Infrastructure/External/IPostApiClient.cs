using Fetcher.Api.Common.Models;
using Fetcher.Api.Models;

namespace Fetcher.Api.Infrastructure.External;

public interface IPostApiClient
{
    Task<PagedResult<ExternalPost>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<ExternalPost> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ExternalPost>> SearchAsync(int? userId, CancellationToken cancellationToken);
}
