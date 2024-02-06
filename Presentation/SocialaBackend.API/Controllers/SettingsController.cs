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
        public async Task<IActionResult> GetDescription()
        {
            return Ok(await _service.GetDescriptionAsync());
        }
        [HttpPost("description")]
        public async Task<IActionResult> PostDescription([FromForm]SettingsDescriptionPostDto dto)
        {
            await _service.PostDescriptionAsync(dto);
            return NoContent();
        }
    }
}
