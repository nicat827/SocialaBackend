using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Infrastructure.Implementations
{
    internal class CloudinaryService : ICloudinaryService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IFileService _fileService;

        public CloudinaryService(IConfiguration configuration, IWebHostEnvironment env, IFileService fileService)
        {
            _configuration = configuration;
            _env = env;
            _fileService = fileService;
        }
        public async Task<string> UploadFileAsync(string imageUrl, params string[] folders)
        {
            var cloudinaryAccount = new Account(_configuration["Cloudinary:CloudName"], _configuration["Cloudinary:ApiKey"], _configuration["Cloudinary:ApiSecret"]);
            var cloudinary = new Cloudinary(cloudinaryAccount);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(_fileService.GeneratePath(imageUrl, "uploads", "users", "avatars")),
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null) throw new CloudinaryFileUploadException(uploadResult.Error.Message);
            return uploadResult.SecureUrl.ToString();
            
            
        }
    }
}
