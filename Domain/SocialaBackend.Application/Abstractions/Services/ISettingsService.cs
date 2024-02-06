using Microsoft.AspNetCore.Http;
using SocialaBackend.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface ISettingsService
    {

        Task<SettingsDescriptionGetDto> GetDescriptionAsync();
        Task<string> ChangeAvatarAsync(IFormFile photo);

        Task<string?> ChangeBioAsync(string? bio);

        Task PostDescriptionAsync(SettingsDescriptionPostDto dto);
    }
}
