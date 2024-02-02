using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;
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
    internal class FileService:IFIleService
    {
        private readonly string _rootPath;
        public FileService(IWebHostEnvironment env)
        {
            _rootPath = env.WebRootPath;

        }
        public void CheckFileType(IFormFile file, FileType type)
        {
            switch (type)
            {
                case FileType.Image:
                    if (!file.ContentType.Contains("image/")) throw new FileValidationException("Invalid upload! We expected a photo!");
                    break;
                case FileType.Audio:
                    if (!file.ContentType.Contains("audio/")) throw new FileValidationException("Invalid upload! We expected an audio!");
                    break;
                case FileType.Video:
                    if (!file.ContentType.Contains("video/")) throw new FileValidationException("Invalid upload! We expected a video!");
                    break;
            }
        }
        public  void CheckFileSize(IFormFile file, int maxSize, FileSize type = FileSize.Mb)
        {
            switch (type)
            {
                case FileSize.Kb:
                    if (file.Length >= maxSize * 1024) throw new FileValidationException($"File length must be less than {maxSize + type}!");
                    break;
                case FileSize.Mb:
                    if (file.Length >= maxSize * 1024 * 1024) throw new FileValidationException($"File length must be less than {maxSize + type}!");
                    break;
                case FileSize.Gb:
                    if (file.Length >= maxSize * 1024 * 1024 * 1024) throw new FileValidationException($"File length must be less than {maxSize + type}!");
                    break;

            }
        }

        public async Task<string> CreateFileAsync(IFormFile file,params string[] folders)
        {
            string fileName = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
            string path = _generatePath(fileName, folders);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }

        public void DeleteFile(string fileName, params string[] folders)
        {
            string path = _generatePath(fileName, folders);
            if (File.Exists(path)) File.Delete(path);
        }
        private string _generatePath(string fileName, params string[] folders)
        {
            string path = _rootPath;
            foreach (var folder in folders)
            {
                path = Path.Combine(path, folder);
            }
            return Path.Combine(path, fileName);
        }

        public void ValidateFilesForPost(IFormFile file)
        {
            if (!file.ContentType.Contains("video/") && !file.ContentType.Contains("image/")) throw new FileValidationException("Invalid upload for creating post!");
        }
    }
}
