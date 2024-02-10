using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Services
{
    internal class StoryService : IStoryService
    {
        private readonly string _currentUsername;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStoriesRepository _storiesRepository;

        public StoryService(ICloudinaryService cloudinaryService, IFileService fileService, IHttpContextAccessor http, IMapper mapper, UserManager<AppUser> userManager, IStoriesRepository storiesRepository)
        {
            _currentUsername = http.HttpContext.User.Identity.Name;
            _cloudinaryService = cloudinaryService;
            _fileService = fileService;
            _mapper = mapper;
            _userManager = userManager;
            _storiesRepository = storiesRepository;
        }
        public async Task CreateStoryItemAsync(StoryItemPostDto dto)
        {
            AppUser? user = await _userManager.Users.Where(u => u.UserName == _currentUsername).Include(u => u.Story).FirstOrDefaultAsync();
            if (user is null) throw new AppUserNotFoundException($"User with username {_currentUsername} wasnt found!");
            StoryItem newStoryItem = new()
            {
                Text = dto.Text,
                Story = user.Story,
            };
            FileType type = _fileService.ValidateFilesForPost(dto.File);
            string url = await _fileService.CreateFileAsync(dto.File, "uploads", "stories");
            string cloudinaryUrl = await _cloudinaryService.UploadFileAsync(url, type, "uploads", "stories");
            newStoryItem.SourceUrl = cloudinaryUrl;
            await _storiesRepository.CreateAsync(newStoryItem);
            await _storiesRepository.SaveChangesAsync();

        }

        public Task<ICollection<StoryItemCurrentGetDto>> GetCurrentUserStoryItemsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<StoryGetDto>> GetStoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<StoryItemGetDto>> GetStoryItemsAsync(int storyId)
        {
            throw new NotImplementedException();
        }

        private async Task<AppUser> _getUser()
        {
            AppUser? user = await _userManager.Users.Where(u => u.UserName == _currentUsername).Include(u => u.Story).FirstOrDefaultAsync();
            if (user is null) throw new AppUserNotFoundException($"User with username {_currentUsername} wasnt found!");
            return user;
        }

        //private async Task<string> _createStoryItem(IFormFile file, AppUser currentUser)
        //{
        //    _fileService.CheckFileSize(file, 100);
        //    if (currentUser.ImageUrl is not null)
        //    {
        //        _fileService.DeleteFile(currentUser.ImageUrl, "uploads", "users", "avatars");
        //    }
        //    string imageUrl = await _fileService.CreateFileAsync(avatar, "uploads", "users", "avatars");
        //    string cloudinaryUrl = await _cloudinaryService.UploadFileAsync(imageUrl, FileType.Image, "uploads", "users", "avatars");
        //    currentUser.ImageUrl = cloudinaryUrl;
        //    return cloudinaryUrl;

        //}
    }
}
