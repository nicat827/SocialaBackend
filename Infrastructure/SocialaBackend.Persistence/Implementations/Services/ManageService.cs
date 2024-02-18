﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Application.Exceptions.Roles;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Services
{
    internal class ManageService : IManageService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string _currentUserName;

        public ManageService(
            IHttpContextAccessor http,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
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
            foreach (string role in await _userManager.GetRolesAsync(user))
            {
                if (!roles.Any(r => r.ToString() == role)) await _userManager.RemoveFromRoleAsync(user, role);
            }
            foreach (UserRole role in roles)
            {
                if (!await _userManager.IsInRoleAsync(user, role.ToString())) await _userManager.AddToRoleAsync(user, role.ToString());
            }
          
        }
  
        public async Task<ManageGetDto> GetManageAsync()
        {
            return new ManageGetDto
            {
                RegisteredUsersCountByGender = new StatOfRegisteredUsersDto
                {
                    MaleCount = await _userManager.Users.Where(u => u.Gender == Gender.Male).CountAsync(),
                    FemaleCount = await _userManager.Users.Where(u => u.Gender == Gender.Female).CountAsync(),
                    OtherCount = await _userManager.Users.Where(u => u.Gender == Gender.None).CountAsync(),
                }
            };
        }


    }
}
