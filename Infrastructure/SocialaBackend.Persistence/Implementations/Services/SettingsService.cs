using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.Settings;
using SocialaBackend.Application.Exceptions;
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
    internal class SettingsService : ISettingsService
    {
        private readonly string _currentUsername;
        //private readonly FollowRepository _followRepository;
        //private readonly FollowerRepository _followerRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationRepository _notificationRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;

        public SettingsService(IHubContext<NotificationHub> hubContext, /*FollowRepository followRepository, FollowerRepository followerRepository,*/ INotificationRepository notificationRepository, IHttpContextAccessor http, ICloudinaryService cloudinaryService, UserManager<AppUser> userManager, IMapper mapper,IEmailService emailService, IFileService fileService)
        {
            _currentUsername = http.HttpContext.User.Identity.Name;
            //_followRepository = followRepository;
            //_followerRepository = followerRepository;
            _hubContext = hubContext;
            _notificationRepository = notificationRepository;
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
            _mapper = mapper;
            _emailService = emailService;
            _fileService = fileService;
        }
        public async Task<SettingsDescriptionGetDto> GetDescriptionAsync()
        {
            AppUser user = await _getUser();
            return _mapper.Map<SettingsDescriptionGetDto>(user);
        }
        public async Task<SettingsSocialGetDto> GetSocialLinksAsync()
        {
            AppUser user = await _getUser();

            return new SettingsSocialGetDto { FacebookLink = user.FacebookLink, GithubLink = user.GithubLink, InstagramLink = user.InstagramLink };
        }
        public async Task<SettingsNotifyGetDto> GetNotifySettingsAsync()
        {
            AppUser user = await _getUser();
            return new SettingsNotifyGetDto { PhotoLikeNotify = user.PhotoLikeNotify, FollowerNotify = user.FollowerNotify, PostLikeNotify = user.PostLikeNotify };
        }

        public async Task<string?> PostDescriptionAsync(SettingsDescriptionPutDto dto)
        {
            AppUser currentUser = await _getUser();
         
            currentUser.Bio = dto.Bio;
            currentUser.Surname = dto.Surname;
            currentUser.Name = dto.Name;
            currentUser.Gender = dto.Gender;
            currentUser.IsPrivate = dto.IsPrivate;
            if (currentUser.Email != dto.Email)
                if (await _userManager.Users.AnyAsync(u => u.Email == dto.Email))
                    throw new UserAlreadyExistException($"User with email: {dto.Email} already exists!");
            string imgUrl = null;
            if (dto.Photo is not null) imgUrl  = await _createAvatar(dto.Photo, currentUser);
            if (currentUser.Email != dto.Email)
            {
                currentUser.Email = dto.Email;
                currentUser.EmailConfirmed = false;

                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(currentUser);
                var validEmailToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));
                string url = $"http://localhost:5173/confirm?token={validEmailToken}&email={currentUser.Email}";
                string body = $"<body>\r\n    <div style=\"margin: 0; padding: 0; font-family: Arial, sans-serif; background-image: url('https://marketplace.canva.com/EAFcuA4ZUpk/1/0/1600w/canva-beige-pastel-terrazzo-abstract-desktop-wallpaper-rtHx0Wpl5Oc.jpg'); background-size: cover; background-position: center; background-repeat: no-repeat; display: flex; justify-content: center; align-items: center; flex-direction: column; height: 100vh; width: 100vw;\">\r\n      <div class=\"container\" style=\"text-align: center; background-color: rgba(255, 255, 255, 0.8); padding: 20px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);\">\r\n        <img src=\"https://demo.foxthemes.net/socialite-v3.0/assets/images/logo.png\" alt=\"Logo\" class=\"logo\" style=\"width: 100px; height: auto; margin-bottom: 20px;\">\r\n        <h2 style=\"color: rgb(88,80,236)\">Hello, {currentUser.UserName}!</h2>\r\n        <div class=\"confirmation-message\" style=\"font-size: 24px; margin-bottom: 20px;\">\r\n          Thank you for using our website!\r\n        </div>\r\n        <img style=\"width: 350px; height:150px\" src=\"https://sendgrid.com/content/dam/sendgrid/legacy/2019/12/confirmation-email-examples.png\" alt=\"\">\r\n        <div style=\"display: flex; justify-content: center; align-items: center; \">\r\n            <div class=\"social-media-icons\" style=\"margin-left:80px;margin-top: 20px; padding: 5px 10px; border-radius:8px; background-color: rgb(88,80,236); width: 150px; cursor:'pointer' \">\r\n              <a href='{url}' style=\" display: inline-block; margin: 0 10px; text-decoration: none;color: rgba(255, 255, 255, 0.8); transition: transform 0.3s ease;\">change email!</a>\r\n            </div>\r\n        </div>\r\n      </div>\r\n    </div>\r\n</body>";
                await _emailService.SendEmailAsync(currentUser.Email, body, "Change Email", true);
            }

            await _userManager.UpdateAsync(currentUser);
            return imgUrl;
        }

        public async Task<string> ChangeAvatarAsync(IFormFile photo)
        {
            AppUser user = await _getUser();
           
            await _createAvatar(photo, user);
            await _userManager.UpdateAsync(user);
            return user.ImageUrl;
        } 
        public async Task<string> ChangeBackgroundAsync(IFormFile photo)
        {
            AppUser currentUser = await _getUser();

            _fileService.CheckFileType(photo, FileType.Image);
            _fileService.CheckFileSize(photo, 2);
            if (currentUser.BackgroundImage is not null)
            {
                _fileService.DeleteFile(currentUser.BackgroundImage, "uploads", "users", "backgrounds");
            }
            string imageUrl = await _fileService.CreateFileAsync(photo, "uploads", "users", "backgrounds");
            string cloudinaryUrl = await _cloudinaryService.UploadFileAsync(imageUrl, FileType.Image, "uploads", "users", "backgrounds");
            currentUser.BackgroundImage = cloudinaryUrl;
            await _userManager.UpdateAsync(currentUser);
            return cloudinaryUrl;

        }
        public async Task<string?> ChangeSocialMediaLinksAsync(SettingsSocialPutDto dto)
        {
            AppUser user = await _getUser();
            user.FacebookLink = dto.FacebookLink;
            user.GithubLink = dto.GithubLink;
            user.InstagramLink = dto.InstagramLink;
            string? img = null;
            if (dto.Photo is not null) img = await _createAvatar(dto.Photo, user);
            await _userManager.UpdateAsync(user);
            return img;
        }

        public async Task<string?> ChangeNotifySettingsAsync(SettingsNotifyPutDto dto)
        {
            AppUser user = await _getUser();
            user.FollowerNotify = dto.FollowerNotify;
            user.PhotoLikeNotify = dto.PhotoLikeNotify;
            user.PostLikeNotify = dto.PostLikeNotify;
            string? img = null;
            if (dto.Photo is not null) img = await _createAvatar(dto.Photo, user);
            await _userManager.UpdateAsync(user);
            return img;

        }
        public async Task LikeAvatarAsync(string username)
        {
            AppUser? user = await _userManager.Users.Where(u => u.UserName == username).FirstOrDefaultAsync();
            if (user is null) throw new AppUserNotFoundException($"User with username {username} wasnt found!");

            if (user.ImageUrl is not null)
            {
                AppUser? currentUser = await _userManager.Users
                    .Where(u => u.UserName == _currentUsername)
                    .Include(u => u.LikedAvatars)
                    .FirstOrDefaultAsync();
                if (currentUser is null) throw new AppUserNotFoundException($"User with username {_currentUsername} wasnt found!");
                AvatarLikeItem? likeItem = currentUser.LikedAvatars.FirstOrDefault(a => a.UserName == user.UserName);
                if (likeItem is not null)
                {
                    currentUser.LikedAvatars.Remove(likeItem);
                }
                else
                {
                    currentUser.LikedAvatars.Add(new AvatarLikeItem { AppUserId = user.Id, UserName = user.UserName });
                    if (user.PhotoLikeNotify && user.UserName != _currentUsername)
                    {
                        Notification newNotification = new Notification
                        {
                            AppUser = user,
                            Title = "Avatar Liked!",
                            Text = $"User {_currentUsername} liked your avatar",
                            SourceUrl = user.ImageUrl
                        };
                        NotificationsGetDto dto = new() { Title = newNotification.Title, Text = newNotification.Text, SourceUrl = newNotification.SourceUrl, CreatedAt = DateTime.Now };
                        await _hubContext.Clients.Group(user.UserName).SendAsync("NewNotification", dto);
                        await _notificationRepository.CreateAsync(newNotification);
                       
                    }
                }
                await _userManager.UpdateAsync(currentUser);
            }
            else throw new NotFoundException("Avatar didnt found");
        }

        public async Task<string?> ChangeBioAsync(string? bio)
        {
            AppUser user = await _getUser();
            user.Bio = bio;
            await _userManager.UpdateAsync(user);
            return bio;

        }

        private async Task<AppUser> _getUser()
        {
            AppUser user = await _userManager.FindByNameAsync(_currentUsername);
            if (user is null) throw new AppUserNotFoundException($"User with username {_currentUsername} wasnt found!");
            return user;
        }

        private async Task<string> _createAvatar(IFormFile avatar, AppUser currentUser)
        {
            _fileService.CheckFileType(avatar, FileType.Image);
            _fileService.CheckFileSize(avatar, 2);
            if (currentUser.ImageUrl is not null)
            {
                _fileService.DeleteFile(currentUser.ImageUrl, "uploads", "users", "avatars");
            }
            string imageUrl = await _fileService.CreateFileAsync(avatar, "uploads", "users", "avatars");
            string cloudinaryUrl = await _cloudinaryService.UploadFileAsync(imageUrl, FileType.Image, "uploads", "users", "avatars");
            //ICollection<FollowerItem> followerItems = await _followerRepository.GetCollection(fi => fi.UserName == currentUser.UserName);
            //ICollection<FollowItem> followItems = await _followRepository.GetCollection(fi => fi.UserName == currentUser.UserName);
            //foreach (var item in followItems) item.ImageUrl = cloudinaryUrl;
            //foreach (var item in followerItems) item.ImageUrl = cloudinaryUrl;
            //await _followerRepository.SaveChangesAsync();
            currentUser.ImageUrl = cloudinaryUrl;
            return cloudinaryUrl;
            
        }

    }
}
