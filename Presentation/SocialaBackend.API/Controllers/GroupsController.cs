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

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromForm]GroupCreateDto dto)
        {
            await _groupService.CreateGroupAsync(dto);
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
