using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;

namespace SocialaBackend.API.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IHttpContextAccessor _http;

        public GroupsController(IGroupService groupService, IHttpContextAccessor http)
        {
            _groupService = groupService;
            _http = http;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromForm]GroupCreateDto dto)
        {
            await _groupService.CreateGroupAsync(dto);
            return StatusCode(StatusCodes.Status201Created);
        }
        [HttpGet("items")]
        [Authorize]
        public async Task<IActionResult> GetGroupItems()
        {

            return Ok(await _groupService.GetGroupItemsAsync(_http.HttpContext.User.Identity.Name));
        }
    }
}
