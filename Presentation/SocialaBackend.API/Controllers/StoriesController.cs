using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;

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

        [HttpGet("currentUserItems")]
        public async Task<IActionResult> GetCurrentUserItems()
        {
            return Ok(await _service.GetCurrentUserStoryItemsAsync());
        }





    }
}
