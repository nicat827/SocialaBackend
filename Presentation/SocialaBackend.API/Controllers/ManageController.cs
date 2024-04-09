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
           
            return Ok(await _service.ChangeRolesUserAsync(userName, roles));
        }

        [HttpPost("verify")]
        [Authorize(Roles = "Member")]

        public async Task<IActionResult> RequestToVerify()
        {
            await _service.AddRequestForVerifyAsync();
            return NoContent();
        }

        [HttpPut("verify/{id}")]
        [Authorize(Roles = "Admin, Moderator")]

        public async Task<IActionResult> ConfirmOrCancelVerifyRequestAsync(int id, bool status)
        {
            if (id <= 0) throw new InvalidIdException("Id cant be negative!");
            await _service.ConfirmOrCancelVerifyRequestAsync(id, status);
            return NoContent();
        }

        [HttpGet("verifyRequests")]
        [Authorize(Roles = "Admin, Moderator")]

        public async Task<IActionResult> GetVerifyRequestsAsync(string sortType, bool desc, int skip)
        {
            if (skip < 0) throw new InvalidSkipException("Skip cant be negative!");
            return Ok(await _service.GetVerifyRequestsAsync(sortType, desc, skip));
        }

        [HttpGet("verifyRequestsCount")]
        [Authorize]

        public async Task<IActionResult> GetCount()
        {
            return Ok(await _service.GetVerifyRequestsCountAsync());
        }

    }
}
