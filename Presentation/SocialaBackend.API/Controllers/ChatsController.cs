using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.Chat.Message;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Application.Exceptions.Chat;

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

        [HttpPost("media")]
        [Authorize]

        public async Task<IActionResult> UploadMedia([FromForm] int chatId, [FromForm] IList<IFormFile> mediaFiles, [FromForm] IList<string>? mediaTexts)
        {
            if (mediaFiles.Count == 0) throw new SendMessageException("You cant send empty message!");

            // Создаем список DTO для передачи сервису
            var mediaDtoList = new List<MediaMessagePostDto>();
            for (int i = 0; i < mediaFiles.Count; i++)
            {
                var mediaDto = new MediaMessagePostDto
                {
                    File = mediaFiles[i],
                    Text = mediaTexts[i] is not null ? mediaTexts[i].Trim().Count() > 0 ? mediaTexts[i] : null : null
                };
                mediaDtoList.Add(mediaDto);
            }

            await _chatService.SendMediaAsync(_http.HttpContext.User.Identity.Name, chatId, mediaDtoList);

            return Ok();
        }


    }
}
