using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;

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
        [HttpPost]
        public async Task<IActionResult> Post([FromForm]AppUserRegisterDto dto)
        {
            return StatusCode(StatusCodes.Status201Created, await _service.RegisterAsync(dto));
        }

        [Authorize]
        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            return Ok(await _service.GetAsync(username));
        }
    }
}
