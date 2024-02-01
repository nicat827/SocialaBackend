using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos.Users;

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
        public async Task<IActionResult> Post(UserPostDto dto)
        {
            return StatusCode(StatusCodes.Status201Created, await _service.RegisterAsync(dto));
        }
    }
}
