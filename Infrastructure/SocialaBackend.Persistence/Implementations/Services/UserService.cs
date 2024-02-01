using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Services
{
    internal class UserService : IUserService
    {
        private readonly IFIleService _fileService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public UserService(IFIleService fileService, UserManager<AppUser> userManager, IMapper mapper, ITokenService tokenService)
        {
            _fileService = fileService;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }
        public async Task<AppUserRegisterResponseDto> RegisterAsync(AppUserRegisterDto dto)
        {
            if (dto.Photo is not null)
            {
                _fileService.CheckFileType(dto.Photo, FileType.Image);
                _fileService.CheckFileSize(dto.Photo, 2);
            }

            if (await _userManager.Users.AnyAsync(u => u.UserName == dto.Username)) throw new UserAlreadyExistException($"User with username {dto.Username} already exists!");
            AppUser newUser = _mapper.Map<AppUser>(dto);
            var res = await _userManager.CreateAsync(newUser, dto.Password);
            if (!res.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (IdentityError err in res.Errors)
                {
                    sb.AppendLine(err.Description);
                }
                throw new AppUserCreateException(sb.ToString());
            }
            if (dto.Photo is not null) newUser.ImageUrl = await _fileService.CreateFileAsync(dto.Photo, "uploads", "users", "avatars");
            await _userManager.AddToRoleAsync(newUser, UserRole.Member.ToString());

            TokenResponseDto tokens = await _tokenService.GenerateTokensAsync(newUser, 15);
            newUser.RefreshToken = tokens.RefreshToken;
            newUser.RefreshTokenExpiresAt = tokens.RefreshTokeExpiresAt;
            await _userManager.UpdateAsync(newUser);
            return new AppUserRegisterResponseDto(newUser.UserName, tokens.AccessToken, tokens.RefreshToken);
        }

    }
}
