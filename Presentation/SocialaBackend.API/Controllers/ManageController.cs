using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Domain.Enums;

namespace SocialaBackend.API.Controllers
{
    [Route("api/manage")]
    [ApiController]

    public class ManageController : ControllerBase
    {
        private readonly IManageService _service;

        public ManageController(IManageService service)
        {
            _service = service;
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.GetManageAsync());
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> SearchUsers(string searchTerm, int skip)
        {
            if (skip < 0) throw new InvalidSkipException("Skip cant be negative!");
            return Ok(await _service.SearchUsersAsync(searchTerm, skip));
        }

        [HttpPut("roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRoles([FromForm]string userName, [FromForm] IEnumerable<UserRole> roles)
        {
            await _service.ChangeRolesUserAsync(userName, roles);
            return NoContent();
        }

    }
}
