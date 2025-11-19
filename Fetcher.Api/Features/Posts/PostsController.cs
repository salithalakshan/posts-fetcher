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

}
