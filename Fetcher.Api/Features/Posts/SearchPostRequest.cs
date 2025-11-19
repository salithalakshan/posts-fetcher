namespace Fetcher.Api.Features.Posts;

public sealed record SearchPostRequest(int? UserId, string Query);