using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using System.Runtime.CompilerServices;

namespace SocialaBackend.API.Controllers
{
    [Route("api/posts")]
    [Authorize]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _service;

        public PostsController(IPostService service)
        {
            _service = service;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromForm]PostPostDto dto)
        {

            await _service.CreatePostAsync(User.Identity?.Name, dto);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            return Ok(await _service.GetPostsAsync(username));
        }

        [HttpPost("{id}/comment")]
        [Authorize]
        public async Task<IActionResult> Post(int id, string text)
        {
            await _service.CommentAsync(id, text, User.Identity?.Name);
            return StatusCode(StatusCodes.Status201Created);

        }

        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> Post(int id)
        {
            await _service.LikePostAsync(id,  User.Identity?.Name);
            return NoContent();

        }
    }
}
