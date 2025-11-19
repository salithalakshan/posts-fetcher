namespace Fetcher.Api.Models;

public sealed record ExternalPost(
    int Id,
    int UserId,
    string Title,
    string Body
    );