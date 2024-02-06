using Microsoft.AspNetCore.Http;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface IFileService
    {
        void CheckFileType(IFormFile file, FileType type);
        void CheckFileSize(IFormFile file, int maxSize, FileSize type = FileSize.Mb);
        FileType ValidateFilesForPost(IFormFile file);

        string GeneratePath(string imageUrl, params string[] folders);
        
        Task<string> CreateFileAsync(IFormFile file, params string[] folders);
        void DeleteFile(string fileName, params string[] folders);
    }
}
