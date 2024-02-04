using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.AppUsers;
using SocialaBackend.Application.Exceptions.Token;
using System.Net;
using System.Runtime.CompilerServices;

namespace SocialaBackend.API.Controllers
{
    [Route("api/")]
    [EnableCors()]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IPostService _postService;

        public UsersController(IUserService service, IPostService postService)
        {
            _service = service;
            _postService = postService;
        }
        [HttpPost("auth/register")]
        public async Task<IActionResult> Post([FromForm]AppUserRegisterDto dto)
        {
            AppUserRegisterResponseDto res = await _service.RegisterAsync(dto);
            return StatusCode(StatusCodes.Status201Created, res);
        }
        [HttpPost("auth/login")]

        public async Task<IActionResult> Post([FromForm]AppUserLoginDto dto)
        {
            AppUserLoginResponseDto res = await _service.LoginAsync(dto);
            return StatusCode(StatusCodes.Status200OK, res);
        }

        [Authorize]
        [HttpGet("auth")]

        public async Task<IActionResult> Get()
        {
            return Ok(await _service.GetCurrentUserAsync(User.Identity.Name));
        }
        [HttpPost("auth/refresh/{refreshToken}")]
        public async Task<IActionResult> Post(string refreshToken)
        {
            TokenResponseDto res = await _service.RefreshAsync(refreshToken);
            return Ok(res);
        }
        [Authorize]
        [HttpPost("auth/logout/{refreshToken}")]
        public async Task<IActionResult> Logout(string refreshToken)
        {
            await _service.LogoutAsync(refreshToken);
            return Ok();
        }

        [Authorize]
        [HttpGet("users/{username}")]
        public async Task<IActionResult> Get(string username)
        {
            return Ok(await _service.GetAsync(username));
        }

        [Authorize]
        [HttpPost("users/{username}/follow")]
        public async Task<IActionResult> Follow(string username)
        {
            await _service.FollowAsync(User.Identity.Name, username);
            return Ok();
        }
        [Authorize]
        [HttpGet("users/{username}/follows")]
        public async Task<IActionResult> GetFollows(string username, int? skip = null)
        {
            return Ok(await _service.GetFollowsAsync(username, skip));
        }
        [Authorize]
        [HttpGet("users/{username}/followers")]
        public async Task<IActionResult> GetFollowers(string username , int? skip = null)
        {
            return Ok(await _service.GetFollowersAsync(username, skip));
        }

        [Authorize]
        [HttpPost("users/followers/confirm/{id}")]
        public async Task<IActionResult> ConfirmFollower(int id)
        {
            await _service.ConfirmFollowerAsync(User.Identity.Name, id);
            return Ok();
        }

        [Authorize]
        [HttpDelete("users/followers/cancel/{id}")]
        public async Task<IActionResult> CancelFollower(int id)
        {
            await _service.CancelFollowerAsync(User.Identity.Name, id);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("users/follows/cancel/{id}")]
        public async Task<IActionResult> CancelFollow(int id)
        {
            await _service.CancelFollowAsync(User.Identity.Name, id);
            return NoContent();
        }


    }
}
