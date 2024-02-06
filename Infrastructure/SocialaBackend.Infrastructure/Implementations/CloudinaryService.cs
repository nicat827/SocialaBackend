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
        private readonly Cloudinary _cloudinary;
        private readonly IFileService _fileService;

        public CloudinaryService(IConfiguration configuration, Cloudinary cloudinary, IFileService fileService)
        {
            _configuration = configuration;
            _cloudinary = cloudinary;
            _fileService = fileService;
        }
        public async Task<string> UploadFileAsync(string imageUrl, params string[] folders)
        {
            

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(_fileService.GeneratePath(imageUrl, folders)),
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null) throw new CloudinaryFileUploadException(uploadResult.Error.Message);
            return uploadResult.SecureUrl.ToString();
            
            
        }
    }
}
