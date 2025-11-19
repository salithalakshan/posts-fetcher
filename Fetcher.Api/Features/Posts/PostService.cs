using Fetcher.Api.Common.Exceptions;
using Fetcher.Api.Common.Models;
using Fetcher.Api.Infrastructure.External;
using Fetcher.Api.Models;
using Fetcher.CacheService.Cache;
using System.Text.Json;

namespace Fetcher.Api.Features.Posts;

public class PostService(
    IPostApiClient postApiClient,
    ICacheStoreService cachedService,
    ILogger<PostService> logger
    ) : IPostService
{
    public readonly IPostApiClient _postApiClient = postApiClient;
    public readonly ICacheStoreService _cachedService = cachedService;
    public readonly ILogger<PostService> _logger = logger;

    public async Task<PagedResult<GetPostResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0 || pageSize > 100) pageSize = 20;

        var cacheKey = $"posts:all:page={page}:pageSize={pageSize}";
        var cachedData = await _cachedService.GetAsync(cacheKey, cancellationToken);

        if(cachedData is not null)
        {
            var cachedPosts = JsonSerializer.Deserialize<IReadOnlyCollection<ExternalPost>>(cachedData);
            _logger.LogInformation("Fetched {Count} posts from cache", cachedPosts.Count);

            return new PagedResult<GetPostResponse>(
                    Items: cachedPosts.Select(GetPostResponse.Map).ToList(),
                    Page: page,
                    PageSize: pageSize,
                    TotalCount: -1,
                    TotalPages: -1
                );
        }

        var externalPosts = await _postApiClient.GetAllAsync(cancellationToken);
        _logger.LogInformation("Fetched {Count} posts from external API", externalPosts.Count);

        var totalCount = externalPosts.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var pagedPosts = externalPosts
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(GetPostResponse.Map)
            .ToList();

        if (externalPosts.Any())
        {
            _ = CacheAsync(cacheKey, pagedPosts, CancellationToken.None)
                .ContinueWith(
                    t => logger.LogError(t.Exception, "Error while caching posts for key {Key}", cacheKey),
                    TaskContinuationOptions.OnlyOnFaulted
                );
        }

        return new PagedResult<GetPostResponse>(
                        Items: pagedPosts,
                        Page: page,
                        PageSize: pageSize,
                        TotalCount: totalCount,
                        TotalPages: totalPages
                    );

    }

    public async Task<GetPostResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var cacheKey = $"posts:{id}";
        var cachedData = await _cachedService.GetAsync(cacheKey, cancellationToken);
        if (cachedData is not null)
        {
            var cachedPost = JsonSerializer.Deserialize<ExternalPost>(cachedData);
            _logger.LogInformation("Fetched post {Id} from cache", id);
            return GetPostResponse.Map(cachedPost);
        }

        var externalPost = await _postApiClient.GetByIdAsync(id, cancellationToken);
        if(externalPost is null)
        {
            throw new NotFoundException($"Post with Id: {id} not found.");
        }

        _ = CacheAsync(cacheKey, externalPost, CancellationToken.None)
                .ContinueWith(
                    t => logger.LogError(t.Exception, "Error while caching posts for key {Key}", cacheKey),
                    TaskContinuationOptions.OnlyOnFaulted
                );

        return GetPostResponse.Map(externalPost);
    }

    public async Task<IReadOnlyCollection<GetPostResponse>> SearchAsync(SearchPostRequest searchPost, CancellationToken cancellationToken)
    {
        var keyUserId = searchPost.UserId.HasValue && searchPost.UserId >= 0 ? searchPost.UserId.ToString() : "null";
        var keyQuery = searchPost.Query;
        var cacheKey = $"posts:search:userId={keyUserId}:query={keyQuery}";

        var cachedData = await _cachedService.GetAsync(cacheKey, CancellationToken.None);
        if(cachedData is not null)
        {
            var cachedPosts = JsonSerializer.Deserialize<IReadOnlyCollection<ExternalPost>>(cachedData);
            _logger.LogInformation("Fetched {Count} posts from cache for search {CacheKey}", cachedPosts.Count, cacheKey);
            return cachedPosts.Select(GetPostResponse.Map).ToList();
        }

        var externalPosts = await _postApiClient.SearchAsync(searchPost.UserId, cancellationToken);
        _logger.LogInformation("Fetched {Count} posts from external API", externalPosts.Count);
        
        var filteredPosts = externalPosts.Where(post =>
                                string.IsNullOrWhiteSpace(searchPost.Query) ||
                                post.Title.Contains(searchPost.Query, StringComparison.OrdinalIgnoreCase)).ToList();

        if (filteredPosts.Any())
        {
            _ = CacheAsync(cacheKey, filteredPosts, CancellationToken.None)
                .ContinueWith(
                    t => logger.LogError(t.Exception, "Error while caching posts for key {Key}", cacheKey),
                    TaskContinuationOptions.OnlyOnFaulted
                );
        }

        return filteredPosts.Select(GetPostResponse.Map).ToList();

    }


    private async Task CacheAsync<T>(string key, T value, CancellationToken cancellationToken)
    {
        var jsonData = JsonSerializer.Serialize(value);
        await _cachedService.AddAsync(key, jsonData, cancellationToken);
    }
}
