using Fetcher.Api.Infrastructure.Configs;
using Fetcher.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
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
            throw new HttpRequestException($"Failed to fetch posts. Status Code: {response.StatusCode}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var externalPosts = await JsonSerializer.DeserializeAsync<IReadOnlyCollection<ExternalPost>>(stream, _serializerOptions,  cancellationToken);
        return externalPosts;
    }

    private  HttpClient Configure(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(_config.BaseUrl.TrimEnd('/'));
        return httpClient;
    }
}
