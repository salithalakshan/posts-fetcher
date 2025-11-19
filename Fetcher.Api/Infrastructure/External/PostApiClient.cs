using Fetcher.Api.Common.Exceptions;
using Fetcher.Api.Infrastructure.Configs;
using Fetcher.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Fetcher.Api.Infrastructure.External;

public sealed class PostApiClient(
    HttpClient httpClient,
    ILogger<PostApiClient> logger,
    IOptions<ExternalApiConfig> externalApiConfigs
    ) : IPostApiClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<PostApiClient> _logger = logger;
    private readonly ExternalApiConfig _config = externalApiConfigs.Value;
    private readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IReadOnlyCollection<ExternalPost>> GetAllAsync(CancellationToken cancellationToken)
    {
        var client = Configure(_httpClient);
        using var response = await client.GetAsync(_config.PostsEndpoint, cancellationToken);

        if(!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch posts. Status Code: {StatusCode}", response.StatusCode);
            throw new ExternalApiException($"Failed to fetch posts. Status Code: {response.StatusCode}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var externalPosts = await JsonSerializer.DeserializeAsync<IReadOnlyCollection<ExternalPost>>(stream, _serializerOptions,  cancellationToken);
        return externalPosts;
    }

    public async Task<ExternalPost> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching post with Id : {id}", id);
        var client = Configure(_httpClient);
        using var response = await client.GetAsync($"{_config.PostsEndpoint}/{id}", cancellationToken);
        if(!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch post with Id: {Id}. Status Code: {StatusCode}", id, response.StatusCode);
            
            Exception ex = response.StatusCode switch
            {
                HttpStatusCode.NotFound => new NotFoundException($"Post with Id: {id} not found."),
                _ => new ExternalApiException($"Failed to fetch post with Id: {id}. Status Code: {response.StatusCode}")
            };

            throw ex;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var externalPost = await JsonSerializer.DeserializeAsync<ExternalPost>(stream, _serializerOptions, cancellationToken);
        return externalPost;

    }

    private  HttpClient Configure(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(_config.BaseUrl.TrimEnd('/'));
        return httpClient;
    }
}
