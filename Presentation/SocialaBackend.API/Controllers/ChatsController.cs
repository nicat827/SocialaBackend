using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos.Chat.Message;
using SocialaBackend.Application.Exceptions;
using static System.Net.WebRequestMethods;

namespace SocialaBackend.API.Controllers
{
    [Route("api/chats")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHttpContextAccessor _http;

        public ChatsController(IChatService chatService, IHttpContextAccessor http)
        {
            _chatService = chatService;
            _http = http;
        }
        [HttpPost("sendAudio")]
        [Authorize]
        public async  Task<IActionResult> SendAudio([FromForm]AudioMessagePostDto dto)
        {
            if (dto.ChatId <= 0) throw new InvalidIdException("Id cant be negative or zero!");
            if (dto.Minutes < 0 || dto.Seconds < 0) throw new InvalidIdException("Audio duration cant be negative!");
            await _chatService.SendAudioAsync(dto);
            return Ok();
        }
        [Authorize]
        [HttpGet("items")]
        public async Task<IActionResult> GetChatItems()
        {
            return Ok(await _chatService.GetChatItemsAsync(_http.HttpContext.User.Identity.Name));
        }


    }
}
