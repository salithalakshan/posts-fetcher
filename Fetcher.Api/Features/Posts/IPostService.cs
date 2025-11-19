namespace Fetcher.Api.Features.Posts;

public interface IPostService
{
    Task<IReadOnlyCollection<GetPostResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<GetPostResponse> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<GetPostResponse>> SearchAsync(SearchPostRequest searchPost, CancellationToken cancellationToken);
}
