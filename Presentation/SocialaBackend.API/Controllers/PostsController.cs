using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Exceptions;
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

            await _service.CreatePostAsync(dto);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) throw new InvalidIdException("Id cant be a negative num!");
            await _service.DeletePostAsync(id);
            return NoContent();
        }
        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            return Ok(await _service.GetPostsAsync(username));
        }
        [HttpPut("recover/{id}")]
        public async Task<IActionResult> Recover(int id)
        {
            await _service.RecoverPostAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/comment")]
        [Authorize]
        public async Task<IActionResult> Post(int id, string text)
        {
            if (id <= 0) throw new InvalidIdException("Id cant be a negative num!");
            await _service.CommentAsync(id, text);
            return StatusCode(StatusCodes.Status201Created);

        }

        [HttpGet("{id}/comments")]
        [Authorize]
        public async Task<IActionResult> GetComments(int id, int? skip = null)
        {
            if (skip < 0) throw new InvalidSkipException($"Invalid skip: {skip}!");
            if (id <= 0) throw new InvalidIdException("Id cant be a negative num!");
            return Ok(await _service.GetCommentsAsync(id, skip));

        }

        [HttpPost("comment/{id}/reply")]
        [Authorize]
        public async Task<IActionResult> Reply(int id, string text)
        {
            if (id <= 0) throw new InvalidIdException("Id cant be a negative num!");
            await _service.ReplyCommentAsync(id, text);
            return StatusCode(StatusCodes.Status201Created);

        }

        [HttpGet("comment/{id}/replies")]
        [Authorize]
        public async Task<IActionResult> GetReplies(int id, int? skip = null)
        {
            if (skip < 0) throw new InvalidSkipException($"Invalid skip: {skip}!");
            if (id <= 0) throw new InvalidIdException("Id cant be a negative num!");

            return Ok(await _service.GetRepliesAsync(id, skip));
        }

        [HttpPost("reply/{id}/like")]
        [Authorize]
        public async Task<IActionResult> LikeReply(int id)
        {
            if (id <= 0) throw new InvalidIdException("Id cant be a negative num!");
            await _service.LikeReplyAsync(id);
            return NoContent();

        }

        [HttpPost("comment/{id}/like")]
        [Authorize]
        public async Task<IActionResult> LikeComment(int id)
        {
            if (id <= 0) throw new InvalidIdException("Id cant be a negative num!");
            await _service.LikeCommentAsync(id);
            return NoContent();

        }

        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> Post(int id)
        {
            if (id <= 0) throw new InvalidIdException("Id cant be a negative num!");
            await _service.LikePostAsync(id);
            return NoContent();

        }

        [HttpGet("{id}/likes")]
        [Authorize]
        public async Task<IActionResult> Get(int id, int? skip= null)
        {
            if (skip < 0) throw new InvalidSkipException($"Invalid skip: {skip}!");
            if (id <= 0) throw new InvalidIdException("Id cant be a negative num!");
            return Ok(await _service.GetLikesAsync(id, skip));

        }

    }
}
