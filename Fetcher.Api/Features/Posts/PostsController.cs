using Fetcher.Api.Common.Api;
using Fetcher.Api.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fetcher.Api.Features.Posts;

[ApiController]
[Route("api/v1/[controller]")]
public class PostsController(
    IPostService postService,
    ILogger<PostsController> logger
    ) : ControllerBase
{
    private readonly IPostService _postService = postService;
    private readonly ILogger<PostsController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received request to fetch all posts");

        var posts = await _postService.GetAllAsync(page, pageSize, cancellationToken);

        var response = ApiResponse<PagedResult<GetPostResponse>>.Success(posts, HttpContext.TraceIdentifier);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to fetch post with ID: {Id}", id);

        var post = await _postService.GetByIdAsync(id, cancellationToken);
        var response = ApiResponse<GetPostResponse>.Success(post, HttpContext.TraceIdentifier);
        return Ok(response);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAsync([FromQuery] int? userId, [FromQuery] string query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to fetch posts with userId={UserId}, query={Query}", userId, query);

        var request = new SearchPostRequest(userId, query);
        var posts = await _postService.SearchAsync(request ,cancellationToken);

        var response = ApiResponse<IReadOnlyCollection<GetPostResponse>>.Success(posts, HttpContext.TraceIdentifier);
        return Ok(response);
    }

    //TODO: Add endpoints for creation -> can demonstrate caching invalidation

}
