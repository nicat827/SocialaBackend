using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.AppUsers;

namespace SocialaBackend.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Post([FromForm]AppUserRegisterDto dto)
        {
            return StatusCode(StatusCodes.Status201Created, await _service.RegisterAsync(dto));
        }
        [HttpPost("login")]

        public async Task<IActionResult> Post([FromForm] AppUserLoginDto dto)
        {
            
            return StatusCode(StatusCodes.Status200OK, await _service.LoginAsync(dto));
        }

        [Authorize]
        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            return Ok(await _service.GetAsync(username));
        }
    }
}
