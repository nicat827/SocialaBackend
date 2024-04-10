using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Application.Exceptions.Roles;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Domain.Enums;
using SocialaBackend.Persistence.Implementations.Hubs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Services
{
    internal class ManageService : IManageService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationRepository _notificationRepository;
        private readonly IVerifyRequestRepository _verifyRequestRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string _currentUserName;

        public ManageService(

            IHttpContextAccessor http,
            IHubContext<NotificationHub> hubContext,
            INotificationRepository notificationRepository,
            IVerifyRequestRepository verifyRequestRepository,
            UserManager<AppUser> userManager,
            ITokenService tokenService,
            RoleManager<IdentityRole> roleManager)
        {
            _hubContext = hubContext;
            _notificationRepository = notificationRepository;
            _verifyRequestRepository = verifyRequestRepository;
            _userManager = userManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
            _currentUserName = http.HttpContext.User.Identity.Name;
        }
        public async Task<ICollection<AppUserSearchDto>> SearchUsersAsync(string searchTerm, int skip)
        {
            ICollection<AppUser> users = await _userManager.Users.Where(
                u => u.UserName != _currentUserName &&
                u.UserName.Contains(searchTerm)
                || u.Name.ToLower().Contains(searchTerm.ToLower())
                || u.Surname.ToLower().Contains(searchTerm.ToLower()))
                .Skip(skip)
                .Take(15)
                .ToListAsync();

            ICollection<AppUserSearchDto> dto = new List<AppUserSearchDto>();

            foreach (AppUser user in users)
            {
                IList<string> userRoles = await _userManager.GetRolesAsync(user);
                dto.Add(new AppUserSearchDto
                {
                    Roles = userRoles,
                    Name = user.Name,
                    Surname = user.Surname,
                    UserName = user.UserName,
                    ImageUrl = user.ImageUrl,
                });
            }
            return dto;
        }

        public async Task ChangeRolesUserAsync(string userName, IEnumerable<UserRole> roles)
        {
            AppUser? user = await _userManager.FindByNameAsync(userName);
            if (user is null) throw new AppUserNotFoundException($"User with username {userName} was not defined!");
            bool hasChanges = false;
            string? accessToken = null;
            foreach (string role in await _userManager.GetRolesAsync(user))
            {
                if (!roles.Any(r => r.ToString() == role))
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                    if (!hasChanges) hasChanges = true;
                    if (role == UserRole.Verified.ToString())
                    {
                        VerifyRequest req = await _verifyRequestRepository.Get(v => v.AppUser.UserName == userName, isTracking:true, includes:"AppUser");
                        if (req is not null)
                        {
                            _verifyRequestRepository.Delete(req);
                            await _verifyRequestRepository.SaveChangesAsync();
    
                        }
                    }
                }
            }
            foreach (UserRole role in roles)
            {
                if (!await _userManager.IsInRoleAsync(user, role.ToString()))
                {
                    if (role == UserRole.Verified)
                    {
                        VerifyRequest request = new VerifyRequest
                        {
                            AppUser = user,
                            Status = VerifyStatus.Verified
                        };
                        await _verifyRequestRepository.CreateAsync(request);
                        await _verifyRequestRepository.SaveChangesAsync();
                    }
                    await _userManager.AddToRoleAsync(user, role.ToString());
                    if (!hasChanges) hasChanges = true;

                }

            }
            if (hasChanges)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiresAt = null;
                await _userManager.UpdateAsync(user);
                

            }
          
        }
        public async Task<IEnumerable<VerifyRequestGetDto>> GetVerifyRequestsAsync(string sortType, bool desc, int skip)
        {
            Expression<Func<VerifyRequest, object>>? order = null;
            switch (sortType)
            {
                case "New":
                    order = vr => vr.CreatedAt;
                    break;
                case "Register Time":
                    order = vr => vr.AppUser.Story.CreatedAt;
                    break;
                case "Followers":
                    order = vr => vr.AppUser.Followers.Count;
                    break;

            }
            if (order is null) order = vr => vr.CreatedAt;
            IEnumerable<VerifyRequest> requests = await _verifyRequestRepository.OrderAndGet(
                order,
                desc,
                vr => vr.Status == VerifyStatus.Pending,
                skip,
                1,
                expressionIncludes: vr=> vr.AppUser.Followers.Where(f => f.IsConfirmed),
                includes:new[] { "AppUser", "AppUser.Story" }).ToListAsync();
           ICollection<VerifyRequestGetDto> dto = new List<VerifyRequestGetDto>();
            foreach (VerifyRequest request in requests)
            {
                dto.Add(new VerifyRequestGetDto
                {
                    Id = request.Id,
                    Fullname = request.AppUser.Name + " " + request.AppUser.Surname,
                    UserName = request.AppUser.UserName,
                    ImageUrl = request.AppUser.ImageUrl,
                    FollowersCount = request.AppUser.Followers.Count,
                    RegisteredAt = request.AppUser.Story.CreatedAt

                });
            }
            return dto;

        }

        public async Task<int> GetVerifyRequestsCountAsync()
        {
            return await _verifyRequestRepository.GetCountAsync(vr => vr.Status == VerifyStatus.Pending);
        }
        public async Task ConfirmOrCancelVerifyRequestAsync(int id, bool status)
        {
            VerifyRequest? request = await _verifyRequestRepository.GetByIdAsync(id, true, includes:"AppUser");
            if (request is null) throw new NotFoundException($"Verify request with id {id} wasnt found!");
            request.Status = status ? VerifyStatus.Verified : VerifyStatus.Canceled;
            if (request.Status == VerifyStatus.Verified) await _userManager.AddToRoleAsync(request.AppUser, VerifyStatus.Verified.ToString());
            Notification newNotification = new Notification
            {
                AppUser = request.AppUser,
                Title = "Verify Answer!",
                Text = status ? "Congratulations! Your verification request has been successfully approved."
                              : "Thank you for submitting a verification request. Unfortunately, we are unable to provide it to you.",
                UserName = request.AppUser.UserName,
                Type = NotificationType.Congrat

            };
            await _notificationRepository.CreateAsync(newNotification);
            await _notificationRepository.SaveChangesAsync();
            NotificationsGetDto dto = new NotificationsGetDto
            {
                CreatedAt = newNotification.CreatedAt,
                Id = newNotification.Id,
                IsChecked = newNotification.IsChecked,
                Text = newNotification.Text,
                Title = newNotification.Title,
                Type = newNotification.Type.ToString(),
                UserName = request.AppUser.UserName,
            };
            await _hubContext.Clients.Group(request.AppUser.UserName).SendAsync("NewNotification", dto);
        }
        public async Task AddRequestForVerifyAsync()
        {
            AppUser? user = await _userManager.FindByNameAsync(_currentUserName);
            if (user is null) throw new AppUserNotFoundException($"User with username {_currentUserName} doesnt exists!");
            VerifyRequest request = await _verifyRequestRepository.Get(vr => vr.AppUserId  == user.Id, isTracking:true, includes:"AppUser");
            if (request is not null && request.Status != VerifyStatus.Canceled)
                throw new UserAlreadyExistException("You already have a request for verify!");
            if (request is not null && request.Status == VerifyStatus.Canceled)
            {
                request.Status = VerifyStatus.Pending;
            } 
            else
            {
                await _verifyRequestRepository.CreateAsync(new VerifyRequest { AppUserId = user.Id, Status = VerifyStatus.Pending });
            }
            await _verifyRequestRepository.SaveChangesAsync();
        }
  
        public async Task<ManageGetDto> GetManageAsync()
        {
            IEnumerable<AppUser> admins = await _userManager.GetUsersInRoleAsync(UserRole.Admin.ToString());
            IEnumerable<AppUser> moderators = await _userManager.GetUsersInRoleAsync(UserRole.Moderator.ToString());
            IEnumerable<AppUser> verified = await _userManager.GetUsersInRoleAsync(UserRole.Verified.ToString());
            return new ManageGetDto
            {
                RegisteredUsersCountByGender = new StatOfRegisteredUsersDto
                {
                    MaleCount = await _userManager.Users.Where(u => u.Gender == Gender.Male).CountAsync(),
                    FemaleCount = await _userManager.Users.Where(u => u.Gender == Gender.Female).CountAsync(),
                    OtherCount = await _userManager.Users.Where(u => u.Gender == Gender.None).CountAsync(),
                },
                AdminsCount = admins.Count(),
                ModeratorsCount = moderators.Count(),
                VerifiedUsersCount  = verified.Count(),
                AllUsersCount =  await _userManager.Users.CountAsync()
            };
        }


    }
}
