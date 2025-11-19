namespace Fetcher.Api.Infrastructure.Configs
{
    public sealed class ExternalApiConfig
    {
       public string BaseUrl { get; init; } = string.Empty;
       public string PostsEndpoint { get; init; } = string.Empty;
    }
}
