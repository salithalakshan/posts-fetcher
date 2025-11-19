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
        return Ok(posts);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id,CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to fetch post with ID: {Id}", id);
        var post = await _postService.GetByIdAsync(id, cancellationToken);
        if (post is null)
        {
            _logger.LogWarning("Post with ID: {Id} not found", id);
            return NotFound();
        }

        return Ok(post);
    }


}
