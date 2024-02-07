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
        Task<string> ChangeBackgroundAsync(IFormFile photo);
        Task<SettingsSocialPutDto> ChangeSocialMediaLinksAsync(SettingsSocialPutDto dto);

        Task<SettingsNotifyPutDto> ChangeNotifySettingsAsync(SettingsNotifyPutDto dto);

        Task<string?> ChangeBioAsync(string? bio);

        Task LikeAvatarAsync(string username);

        Task PostDescriptionAsync(SettingsDescriptionPutDto dto);
    }
}
