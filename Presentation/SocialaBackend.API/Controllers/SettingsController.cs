using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;

namespace SocialaBackend.API.Controllers
{
    [Route("api/settings")]
    [EnableCors()]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _service;

        public SettingsController(ISettingsService service)
        {
            _service = service;
        }
        [HttpGet("description")]
        [Authorize]
        public async Task<IActionResult> GetDescription()
        {
            return Ok(await _service.GetDescriptionAsync());
        }
        [HttpPost("description")]
        [Authorize]
        public async Task<IActionResult> PostDescription([FromForm]SettingsDescriptionPutDto dto)
        {
            
            return Ok(await _service.PostDescriptionAsync(dto));
        }
        [HttpPut("photo")]
        [Authorize]
        public async Task<IActionResult> ChangeAvatar(IFormFile photo)
        {
            return Ok(await _service.ChangeAvatarAsync(photo));
        }
        [HttpPut("bio")]
        [Authorize]
        public async Task<IActionResult> ChangeBio([FromForm]string? bio)
        {
            return Ok(await _service.ChangeBioAsync(bio));
        }

        [HttpPut("background")]
        [Authorize]
        public async Task<IActionResult> UploadBackground(IFormFile photo)
        {
            return Ok(await _service.ChangeBackgroundAsync(photo));
        }

        [HttpPut("social")]
        [Authorize]

        public async Task<IActionResult> ChangeSocialLinks([FromForm]SettingsSocialPutDto dto)
        {
            return Ok(await _service.ChangeSocialMediaLinksAsync(dto));
        }
        [HttpGet("social")]
        [Authorize]
        public async Task<IActionResult> GetSocial()
        {
            return Ok(await _service.GetSocialLinksAsync());
        }
        [HttpPut("notification")]
        [Authorize]

        public async Task<IActionResult> ChangeNotificationSettings([FromForm]SettingsNotifyPutDto dto)
        {
            return Ok(await _service.ChangeNotifySettingsAsync(dto));
        }

     
        [HttpGet("notification")]
        [Authorize]
        public async Task<IActionResult> GetNotifySettings()
        {
            return Ok(await _service.GetNotifySettingsAsync());
        }
        [HttpPut("likePhoto")]
        [Authorize]

        public async Task<IActionResult> LikePhoto([FromForm]string username)
        {
            await _service.LikeAvatarAsync(username);
            return NoContent();
        }




    }
}
