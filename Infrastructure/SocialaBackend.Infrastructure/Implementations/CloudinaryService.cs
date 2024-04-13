using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Infrastructure.Implementations
{
    internal class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IFileService _fileService;

        public CloudinaryService(Cloudinary cloudinary, IFileService fileService)
        {
            _cloudinary = cloudinary;
            _fileService = fileService;
        }
        public async Task<string> UploadFileAsync(string srcUrl, FileType type,  params string[] folders)
        {
            RawUploadParams uploadParams;
            switch (type)
            {
                case FileType.Image:
                    uploadParams = new ImageUploadParams()
                    {
                       
                        File = new FileDescription(_fileService.GeneratePath(srcUrl, folders)),
                        
                        
                    };
                    break;
                case FileType.Video:
                    uploadParams = new VideoUploadParams()
                    {
                        File = new FileDescription(_fileService.GeneratePath(srcUrl, folders)),
                    };
                    break;
                case FileType.Audio:
                    uploadParams = new RawUploadParams()
                    {
                        File = new FileDescription(_fileService.GeneratePath(srcUrl, folders)),
                        
                    };
                    break;
                default: throw new CloudinaryFileUploadException("Upload type of post is not supported!");

            }
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null) throw new CloudinaryFileUploadException(uploadResult.Error.Message);
            _fileService.DeleteFile(srcUrl, folders);
            return uploadResult.SecureUrl.ToString();
            
            
        }

        public async Task<string> UploadAudioAsync(IFormFile audioFile)
        {
            using (var stream = audioFile.OpenReadStream())
            {
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(audioFile.FileName, stream)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    // Обработка ошибки загрузки на Cloudinary
                    throw new CloudinaryFileUploadException(uploadResult.Error.Message);
                }

                // Возвращаем URL загруженного файла
                return uploadResult.SecureUrl.ToString();
            }
        }

        public async Task<double> GetAudioDurationAsync(string audioUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                double duration = 0;
                try
                {
                    HttpResponseMessage response = await client.GetAsync(audioUrl);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Разбираем JSON-ответ для получения метаданных
                    JObject metadata = JObject.Parse(responseBody);

                    double durationInSeconds = (double)metadata["duration"];
                    duration = durationInSeconds;
                    Console.WriteLine($"Длительность аудиофайла: {durationInSeconds} секунд");

                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Ошибка при отправке запроса: {e.Message}");
                }
                return duration;
            }
        }  
    }
}
