using Fetcher.Api.Infrastructure.External;

namespace Fetcher.Api.Features.Posts;

public interface IPostService
{
    Task<IReadOnlyCollection<GetPostResponse>> GetAllAsync(CancellationToken cancellationToken);
}

public class PostService(
    IPostApiClient postApiClient,
    ILogger<PostService> logger
    ) : IPostService
{
    public readonly IPostApiClient _postApiClient = postApiClient;
    public readonly ILogger<PostService> _logger = logger;
    public async Task<IReadOnlyCollection<GetPostResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var externalPosts = await _postApiClient.GetAllAsync(cancellationToken);
        _logger.LogInformation("Fetched {Count} posts from external API", externalPosts.Count);

        return externalPosts.Select(GetPostResponse.Map).ToList();
    }
}
