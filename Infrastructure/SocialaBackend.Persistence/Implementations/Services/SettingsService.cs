using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Domain.Entities.User;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public SettingsService(IHttpContextAccessor http, UserManager<AppUser> userManager, IMapper mapper,IEmailService emailService)
        {
            _currentUsername = http.HttpContext.User.Identity.Name;
            _userManager = userManager;
            _mapper = mapper;
            _emailService = emailService;
        }
        public async Task<SettingsDescriptionGetDto> GetDescriptionAsync()
        {
            AppUser user = await _getUser();
            return _mapper.Map<SettingsDescriptionGetDto>(user);
        }

        public async Task PostDescriptionAsync(SettingsDescriptionPostDto dto)
        {
            AppUser currentUser = await _getUser();
            if (currentUser.UserName != dto.UserName)
            {
                if (await _userManager.Users.AnyAsync(u => u.UserName == dto.UserName)) 
                    throw new UserAlreadyExistException($"User with username: {dto.UserName} already exists!");
                currentUser.UserName = dto.UserName;
            }
            currentUser.Bio = dto.Bio;
            currentUser.Gender = dto.Gender;
            if (currentUser.Email != dto.Email)
            {
                if (await _userManager.Users.AnyAsync(u => u.Email == dto.Email))
                    throw new UserAlreadyExistException($"User with email: {dto.Email} already exists!");
                currentUser.Email = dto.Email;
                currentUser.EmailConfirmed = false;

                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(currentUser);
                var validEmailToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));
                string url = $"http://localhost:5173/confirm?token={validEmailToken}&email={currentUser.Email}";
                string body = $"<body>\r\n    <div style=\"margin: 0; padding: 0; font-family: Arial, sans-serif; background-image: url('https://marketplace.canva.com/EAFcuA4ZUpk/1/0/1600w/canva-beige-pastel-terrazzo-abstract-desktop-wallpaper-rtHx0Wpl5Oc.jpg'); background-size: cover; background-position: center; background-repeat: no-repeat; display: flex; justify-content: center; align-items: center; flex-direction: column; height: 100vh; width: 100vw;\">\r\n      <div class=\"container\" style=\"text-align: center; background-color: rgba(255, 255, 255, 0.8); padding: 20px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);\">\r\n        <img src=\"https://demo.foxthemes.net/socialite-v3.0/assets/images/logo.png\" alt=\"Logo\" class=\"logo\" style=\"width: 100px; height: auto; margin-bottom: 20px;\">\r\n        <h2 style=\"color: rgb(88,80,236)\">Hello, {currentUser.UserName}!</h2>\r\n        <div class=\"confirmation-message\" style=\"font-size: 24px; margin-bottom: 20px;\">\r\n          Thank you for using our website!\r\n        </div>\r\n        <img style=\"width: 350px; height:150px\" src=\"https://sendgrid.com/content/dam/sendgrid/legacy/2019/12/confirmation-email-examples.png\" alt=\"\">\r\n        <div style=\"display: flex; justify-content: center; align-items: center; \">\r\n            <div class=\"social-media-icons\" style=\"margin-left:80px;margin-top: 20px; padding: 5px 10px; border-radius:8px; background-color: rgb(88,80,236); width: 150px; cursor:'pointer' \">\r\n              <a href='{url}' style=\" display: inline-block; margin: 0 10px; text-decoration: none;color: rgba(255, 255, 255, 0.8); transition: transform 0.3s ease;\">change email!</a>\r\n            </div>\r\n        </div>\r\n      </div>\r\n    </div>\r\n</body>";
                await _emailService.SendEmailAsync(currentUser.Email, body, "Change Email", true);
            }

            await _userManager.UpdateAsync(currentUser);
        }

        private async Task<AppUser> _getUser()
        {
            AppUser user = await _userManager.FindByNameAsync(_currentUsername);
            if (user is null) throw new AppUserNotFoundException($"User with username {_currentUsername} wasnt found!");
            return user;
        }
    }
}
