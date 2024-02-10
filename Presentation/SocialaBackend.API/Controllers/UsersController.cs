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

        public UsersController(IUserService service)
        {
            _service = service;
        }
        [HttpPost("auth/register")]
        public async Task<IActionResult> Post([FromForm]AppUserRegisterDto dto)
        {
            await _service.RegisterAsync(dto);
            return StatusCode(StatusCodes.Status201Created);
        }
        [HttpPost("auth/confirm")]
        public async Task<IActionResult> Confirm([FromForm] AppUserConfirmEmailDto dto)
        {
           
            return StatusCode(StatusCodes.Status201Created, await _service.ConfirmEmailAsync(dto));
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
            return Ok(await _service.GetCurrentUserAsync());
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
            return Ok(await _service.FollowAsync(username));
        }
        [Authorize]
        [HttpGet("users/{username}/follows")]
        public async Task<IActionResult> GetFollows(string username, int? skip = null)
        {
            return Ok(await _service.GetFollowsAsync(username,skip));
        }
        [Authorize]
        [HttpGet("users/{username}/followers")]
        public async Task<IActionResult> GetFollowers(string username, int? skip = null)
        {
            return Ok(await _service.GetFollowersAsync(username,skip));
        }

        [Authorize]
        [HttpPost("users/followers/confirm/{id}")]
        public async Task<IActionResult> ConfirmFollower(int id)
        {
            await _service.ConfirmFollowerAsync(id);
            return Ok();
        }
        [HttpPost("users/reset")]
        public async Task<IActionResult> ResetPassword([FromForm] string email)
        {
            await _service.ResetPasswordAsync(email);
            return Ok();
        }
        [HttpPut("users/newPassword")]
        public async Task<IActionResult> NewPassword([FromForm] AppUserResetPasswordDto dto)
        {
            return Ok(await _service.SetNewPasswordAsync(dto));
        }
        [HttpGet("users/checkPrivate/{username}")]
        [Authorize]
        public async Task<IActionResult> GetType(string username)
        {
            return Ok(await _service.IsPrivateAsync(username));
        }
        [HttpDelete("users/followers/cancel/{username}")]
        public async Task<IActionResult> CancelFollower(string username)
        {
            await _service.CancelFollowerAsync(username);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("users/follows/cancel/{username}")]
        public async Task<IActionResult> CancelFollow(string username)
        {
            await _service.CancelFollowAsync(username);
            return NoContent();
        }


    }
}
