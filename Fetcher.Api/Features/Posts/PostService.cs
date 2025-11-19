using Fetcher.Api.Common.Exceptions;
using Fetcher.Api.Infrastructure.External;

namespace Fetcher.Api.Features.Posts;

public interface IPostService
{
    Task<IReadOnlyCollection<GetPostResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<GetPostResponse> GetByIdAsync(int id, CancellationToken cancellationToken);
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

    public async Task<GetPostResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var externalPost = await _postApiClient.GetByIdAsync(id, cancellationToken);
        if(externalPost is null)
        {
            throw new NotFoundException($"Post with Id: {id} not found.");
        }
        return GetPostResponse.Map(externalPost);
    }
}
