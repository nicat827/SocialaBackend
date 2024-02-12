using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Application.Exceptions.Forbidden;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Domain.Enums;
using SocialaBackend.Persistence.Implementations.Hubs;
using SocialaBackend.Persistence.Implementations.Repositories;
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
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationRepository _notificationRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStoryRepository _storyRepository;
        private readonly IStoryItemsRepository _storyItemsRepository;

        public StoryService(IHubContext<NotificationHub> hubContext, INotificationRepository notificationRepository, ICloudinaryService cloudinaryService, IFileService fileService, IHttpContextAccessor http, IMapper mapper, UserManager<AppUser> userManager, IStoryItemsRepository storyItemsRepository, IStoryRepository storyRepository)
        {
            _currentUsername = http.HttpContext.User.Identity.Name;
            _hubContext = hubContext;
            _notificationRepository = notificationRepository;
            _cloudinaryService = cloudinaryService;
            _fileService = fileService;
            _mapper = mapper;
            _userManager = userManager;
            _storyItemsRepository = storyItemsRepository;
            _storyRepository = storyRepository;
        }
        public async Task CreateStoryItemAsync(StoryItemPostDto dto)
        {
            AppUser user = await _getUser();
            StoryItem newStoryItem = new()
            {
                Text = dto.Text,
                Story = user.Story
            };

            FileType type = _fileService.ValidateFilesForPost(dto.File);
            string url = await _fileService.CreateFileAsync(dto.File, "uploads", "stories");
            string cloudinaryUrl = await _cloudinaryService.UploadFileAsync(url, type, "uploads", "stories");
            newStoryItem.SourceUrl = cloudinaryUrl;
            newStoryItem.Type = type;
            await _storyItemsRepository.CreateAsync(newStoryItem);
            await _storyItemsRepository.SaveChangesAsync();
            user.Story.LastItemAddedAt = newStoryItem.CreatedAt;
            await _storyItemsRepository.SaveChangesAsync();
        }

        public async Task<ICollection<StoryItemCurrentGetDto>> GetCurrentUserStoryItemsAsync()
        {
            AppUser? user =  await _userManager.Users
                .Where(u => u.UserName == _currentUsername)
                .Include(u => u.Story)
                    .ThenInclude(s => s.StoryItems.Where(si => si.CreatedAt.AddDays(1) > DateTime.Now && si.IsDeleted == false))
                .FirstOrDefaultAsync();
            var orderedItems = user.Story.StoryItems.OrderBy(s => s.CreatedAt);
            if (user is null) throw new AppUserNotFoundException($"User with username {_currentUsername} wasnt found!");
            return _mapper.Map<ICollection<StoryItemCurrentGetDto>>(orderedItems);
        }

        public async Task<IEnumerable<StoryGetDto>> GetStoriesAsync()
        {
            AppUser? user = await _userManager.Users
                .Where(u => u.UserName == _currentUsername)
                    .Include(u => u.Follows.Where(uf => uf.IsConfirmed == true))
                .FirstOrDefaultAsync();
            ICollection<StoryGetDto> dto = new List<StoryGetDto>();
            foreach (FollowItem userFollow in user.Follows)
            {
                Story story = await _storyRepository.Get(s => s.Owner.UserName == userFollow.UserName, includes: new[] { "Owner"});
                if (story == null) throw new NotFoundException("User story is not defined!");
                if (story.LastItemAddedAt?.AddDays(1) > DateTime.Now)
                {
                     dto.Add(new StoryGetDto {
                        Id = story.Id,
                        OwnerImageUrl=userFollow.ImageUrl,
                        OwnerUserName = userFollow.UserName,
                        LastStoryPostedAt = story.LastItemAddedAt
                    });
                }
            }
            var sortedDto = dto.OrderByDescending(s => s.LastStoryPostedAt);
            return sortedDto;

        }
        
        public async Task SoftRemoveStoryItemAsync(int id)
        {
            StoryItem item = await _storyItemsRepository.GetByIdAsync(id, true, iqnoreQuery: true, includes:new[] { "Watchers", "Story", "Story.Owner" });
            if (item is null) throw new NotFoundException($"Story item with id {id} wasnt found!");
            if (item.Story.Owner.UserName != _currentUsername) throw new DontHavePermissionException("You cant delete story item another user!");
            if (item.IsDeleted)
            {
                _storyItemsRepository.Delete(item);
                item.Watchers.Clear();
            }
            else
            {
                _storyItemsRepository.SoftDelete(item);
                Notification newNotification = new Notification
                {
                    AppUser = await _getUser(),
                    Title = "Story added to archive!",
                    Text = $"Your story has been succesufully added to archive!",
                    SourceUrl = item.SourceUrl
                };
                NotificationsGetDto dto = new() { Title = newNotification.Title, Text = newNotification.Text, SourceUrl = newNotification.SourceUrl, CreatedAt = DateTime.Now };
                await _hubContext.Clients.Group(_currentUsername).SendAsync("NewNotification", dto);
                await _notificationRepository.CreateAsync(newNotification);
                foreach (StoryItemWatcher watcher in item.Watchers) watcher.IsDeleted = true;
            }
            await _storyItemsRepository.SaveChangesAsync();

        }
        public async Task<ICollection<StoryItemGetDto>> GetStoryItemsAsync(int storyId)
        {
            Story story = await _storyRepository.GetByIdAsync(storyId, expressionIncludes: s => s.StoryItems.Where(si => si.CreatedAt.AddDays(1) > DateTime.Now),
                            includes: new[] {"Owner", "Owner.Followers" });
            if (story is null) throw new NotFoundException($"Story with id {storyId} wasnt found!");
            if (story.Owner.IsPrivate)
            {
                if (!story.Owner.Followers.Any(f => f.UserName == _currentUsername && f.IsConfirmed == true))
                    throw new ForbiddenException("This account is private, follow for seeing stories!");
            }
            var orderedItems = story.StoryItems.OrderBy(si => si.CreatedAt);
            return _mapper.Map<ICollection<StoryItemGetDto>>(orderedItems);

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
