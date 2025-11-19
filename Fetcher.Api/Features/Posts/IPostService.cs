using Fetcher.Api.Common.Models;

namespace Fetcher.Api.Features.Posts;

public interface IPostService
{
    Task<PagedResult<GetPostResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<GetPostResponse> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<GetPostResponse>> SearchAsync(SearchPostRequest searchPost, CancellationToken cancellationToken);
}
