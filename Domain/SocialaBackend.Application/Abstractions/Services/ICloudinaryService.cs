using Microsoft.AspNetCore.Http;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadFileAsync(string srcUrl,FileType type, params string[] folders);
        Task<string> UploadAudioAsync(IFormFile audioFile);

        Task<double> GetAudioDurationAsync(string audioUrl);
    }
}
