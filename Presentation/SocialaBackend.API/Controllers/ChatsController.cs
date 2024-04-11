using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos.Chat.Message;
using SocialaBackend.Application.Exceptions;

namespace SocialaBackend.API.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatsController(IChatService chatService)
        {
            _chatService = chatService;
        }
        [HttpPost("sendAudio")]
        [Authorize]
        public async  Task<IActionResult> SendAudio([FromForm]AudioMessagePostDto dto)
        {
            if (dto.ChatId <= 0) throw new InvalidIdException("Id cant be negative or zero!");
            await _chatService.SendAudioAsync(dto.Audio, dto.ChatId);
            return Ok();
        }
    }
}
