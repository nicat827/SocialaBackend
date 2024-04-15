using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
    [EnableCors]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _service;

        public PostsController(IPostService service)
        {
            _service = service;
        }
       
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromForm]PostPostDto dto)
        {

            
            return StatusCode(StatusCodes.Status201Created, await _service.CreatePostAsync(dto));
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
        [Authorize]
        public async Task<IActionResult> Get(string username)
        {
            return Ok(await _service.GetPostsAsync(username));
        } 
        [HttpGet("archive")]
        [Authorize]
        public async Task<IActionResult> GetArchive(int skip)
        {
            return Ok(await _service.GetArchivedPostsAsync(skip));
        }
        [HttpGet("feed/{skip}")]
        [Authorize]
        public async Task<IActionResult> GetFeed(int skip)
        {
            return Ok(await _service.GetFeedPostsAsync(skip));
        }
        [HttpPut("recover/{id}")]
        [Authorize]
        public async Task<IActionResult> Recover(int id)
        {
            await _service.RecoverPostAsync(id);
            return NoContent();
        }

        [HttpPost("comment")]
        [Authorize]
        public async Task<IActionResult> Post(CommentPostDto dto)
        {
            if (dto.Id <= 0) throw new InvalidIdException("Id cant be a negative num!");
            return StatusCode(StatusCodes.Status201Created, await _service.CommentAsync(dto));

        }

        [HttpGet("{id}/comments")]
        [Authorize]
        public async Task<IActionResult> GetComments(int id, int? skip = null)
        {
            if (skip < 0) throw new InvalidSkipException($"Invalid skip: {skip}!");
            if (id <= 0) throw new InvalidIdException("Id cant be a negative num!");
            return Ok(await _service.GetCommentsAsync(id, skip));

        }

        [HttpPost("comment/reply")]
        [Authorize]
        public async Task<IActionResult> Reply(ReplyPostDto dto)
        {
            if (dto.Id <= 0) throw new InvalidIdException("Id cant be a negative num!");
            
            return StatusCode(StatusCodes.Status201Created, await _service.ReplyCommentAsync(dto));

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

        [HttpDelete("comment/{id}")]
        [Authorize]

        public async Task<IActionResult> DeleteComment(int id)
        {
            await _service.DeleteCommentAsync(id);
            return NoContent();
        }

    }
}
