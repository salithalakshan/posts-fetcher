using Fetcher.Api.Common.Api;
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
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to fetch all posts");

        var posts = await _postService.GetAllAsync(cancellationToken);

        var response = ApiResponse<IReadOnlyCollection<GetPostResponse>>.Success(posts, HttpContext.TraceIdentifier);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id,CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to fetch post with ID: {Id}", id);
        var post = await _postService.GetByIdAsync(id, cancellationToken);
        if (post is null)
        {
            var errorResponse = ApiResponse<GetPostResponse>.Error(
                code: "PostNotFound", 
                message: $"Post with ID {id} was not found.",
                traceId: HttpContext.TraceIdentifier);
            return NotFound(errorResponse);
        }

        var response = ApiResponse<GetPostResponse>.Success(post, HttpContext.TraceIdentifier);
        return Ok(response);
    }
}
