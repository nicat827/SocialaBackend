using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Exceptions;

namespace SocialaBackend.API.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IGroupService _groupService;
        private readonly IHttpContextAccessor _http;

        public MessagesController(IChatService chatService, IGroupService groupService, IHttpContextAccessor http)
        {
            _chatService = chatService;
            _groupService = groupService;
            _http = http;
        }
        [HttpGet("count")]
        [Authorize]


        public async Task<IActionResult> GetChatsAndGroupsCount()
        {
            string userName = _http.HttpContext.User.Identity.Name;
            if (userName is not null)
            {
                int chatsCount = await _chatService.GetChatsCountAsync(userName);
                int groupsCount = await _groupService.GetGroupsCountAsync(userName);
                return Ok(new { GroupsCount = groupsCount, ChatsCount = chatsCount });
            }
            throw new AppUserNotFoundException($"User didnt found!");
        }
    }
}
