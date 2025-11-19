using Fetcher.Api.Models;

namespace Fetcher.Api.Infrastructure.External;

public interface IPostApiClient
{
    Task<IReadOnlyCollection<ExternalPost>> GetAllAsync(CancellationToken cancellationToken);
}
