using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Exceptions;

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
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchUsers(string searchTerm, int skip)
        {
            if (skip < 0) throw new InvalidSkipException("Skip cant be negative!");
            return Ok(await _service.SearchUsersAsync(searchTerm, skip));
        }
    }
}
