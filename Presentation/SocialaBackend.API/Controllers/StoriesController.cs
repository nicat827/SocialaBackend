using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Exceptions;

namespace SocialaBackend.API.Controllers
{
    [Route("api/stories")]
    [Authorize]
    [ApiController]
    [EnableCors]
    public class StoriesController: ControllerBase
    {
        private readonly IStoryService _service;

        public StoriesController(IStoryService service)
        {
            _service = service;
        }
        [HttpPost]
        public async Task<IActionResult> CreateStoryItem([FromForm]StoryItemPostDto dto)
        {
            await _service.CreateStoryItemAsync(dto);
            return StatusCode(StatusCodes.Status201Created);
        }

        // метод для фида (получение историй тех на кого подписан)
        [HttpGet]

        public async Task<IActionResult> GetStories()
        {
            return Ok(await _service.GetStoriesAsync());
        }
        // получение самих айтемов историй (при клике)

        [HttpGet("items/{storyId}")]
        public async Task<IActionResult> GetStorieItems(int storyId)
        {
            return Ok(await _service.GetStoryItemsAsync(storyId));
        }
        [HttpDelete("item/{id}")]

        public async Task<IActionResult> SoftRemoveStoryItem(int id)
        {
            if (id <= 0) throw new InvalidIdException("Invalid id!");
            await _service.SoftRemoveStoryItemAsync(id);
            return NoContent();
        }


        [HttpGet("currentUserItems")]
        public async Task<IActionResult> GetCurrentUserItems()
        {
            return Ok(await _service.GetCurrentUserStoryItemsAsync());
        }

        [HttpPost("watch/{id}")]
        [Authorize]
        public async Task<IActionResult> WatchStoryItem(int id)
        {
            if (id <= 0) throw new InvalidIdException("Invalid id!");
            await _service.WatchStoryItemAsync(id);
            return NoContent();
        }

        [HttpGet("watchers/{id}")]
        [Authorize]
        public async Task<IActionResult> GetStoryItemWatchers(int id)
        {
            if (id <= 0) throw new InvalidIdException("Invalid id!");
            return Ok(await _service.GetStoryItemWatchersAsync(id));
        }

        [HttpGet("archive")]
        [Authorize]

        public async Task<IActionResult> GetStoriesArchive(int skip)
        {
            if (skip < 0) throw new InvalidSkipException($"Skip cant be negative!");
            return Ok(await _service.GetArchivedStoryItemsAsync(skip));
        }
    }
}
