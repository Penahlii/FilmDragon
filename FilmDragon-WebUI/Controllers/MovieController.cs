using FilmDragon_WebUI.Models;
using FilmDragon_WebUI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FilmDragon_WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly RedisService _redisService;
    private readonly IQueueService _queueService;

    public MovieController(RedisService redisService, IQueueService queueService)
    {
        _redisService = redisService;
        _queueService = queueService;
    }

    [HttpGet("new-posters")]
    public async Task<IActionResult> GetNewPosters()
    {
        var newPosters = await _redisService.GetNewPostersAsync();
        return Ok(newPosters);
    }

    [HttpPost("add-to-queue")]
    public async Task<IActionResult> AddToQueue([FromBody] FilmRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FilmName))
            return BadRequest("Film name is required.");

        try
        {
            // Use QueueService to send the film name to the queue
            await _queueService.SendMessageAsync(request.FilmName);
            return Ok("Film added to the queue successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}
