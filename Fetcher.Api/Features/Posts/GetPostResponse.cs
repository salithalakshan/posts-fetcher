using Fetcher.Api.Models;

namespace Fetcher.Api.Features.Posts;

public sealed record GetPostResponse(
    int Id,
    int UserId,
    string Title,
    string Body
    )
{
    public static GetPostResponse Map(ExternalPost externalPost) =>
        new(
            externalPost.Id,
            externalPost.UserId,
            externalPost.Title,
            externalPost.Body
        );
}