using Microsoft.AspNetCore.Http;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.Settings;
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
        Task<SettingsSocialGetDto> GetSocialLinksAsync();
        Task<SettingsNotifyGetDto> GetNotifySettingsAsync();
        Task<string> ChangeAvatarAsync(IFormFile photo);
        Task<string> ChangeBackgroundAsync(IFormFile photo);
        Task<string?> ChangeSocialMediaLinksAsync(SettingsSocialPutDto dto);

        Task<string?> ChangeNotifySettingsAsync(SettingsNotifyPutDto dto);

        Task<string?> ChangeBioAsync(string? bio);

        Task LikeAvatarAsync(string username);

        Task<string?> PostDescriptionAsync(SettingsDescriptionPutDto dto);
    }
}
