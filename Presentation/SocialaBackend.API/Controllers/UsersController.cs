using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.AppUsers;
using SocialaBackend.Application.Exceptions.Token;

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
            HttpContext.Response.Cookies.Append("refreshToken", JsonConvert.SerializeObject(res.RefreshToken));
            return StatusCode(StatusCodes.Status201Created, res);
        }
        [HttpPost("auth/login")]

        public async Task<IActionResult> Post([FromForm] AppUserLoginDto dto)
        {
            AppUserLoginResponseDto res = await _service.LoginAsync(dto);
            HttpContext.Response.Cookies.Append("refreshToken", JsonConvert.SerializeObject(res.RefreshToken));
            return StatusCode(StatusCodes.Status200OK, res);
        }
        [HttpPost("auth/refresh")]
        [Authorize]
        public async Task<IActionResult> Post()
        {
            if (HttpContext.Request.Cookies["refreshToken"] is null) throw new InvalidTokenException("Refresh token is null!");
            string refreshToken = JsonConvert.DeserializeObject<string>(HttpContext.Request.Cookies["refreshToken"]);
            TokenResponseDto res = await _service.RefreshAsync(refreshToken);
            HttpContext.Response.Cookies.Append("refreshToken", JsonConvert.SerializeObject(res.RefreshToken));
            return Ok(res);
        }

        [Authorize]
        [HttpPost("auth/logout")]
        public async Task<IActionResult> Logout()
        {
            if (HttpContext.Request.Cookies["refreshToken"] is null) throw new InvalidTokenException("Refresh token is null!");
            string refreshToken = JsonConvert.DeserializeObject<string>(HttpContext.Request.Cookies["refreshToken"]);
            await _service.LogoutAsync(refreshToken);
            return Ok();
        }

        [Authorize]
        [HttpGet("users/{username}")]
        public async Task<IActionResult> Get(string username)
        {
            return Ok(await _service.GetAsync(username));
        }

    }
}
