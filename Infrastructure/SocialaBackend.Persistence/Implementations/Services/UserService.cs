using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.AppUsers;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Application.Exceptions.AppUser;
using SocialaBackend.Application.Exceptions.Token;
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

        public async Task FollowAsync(string followerUsername, string followToUsername)
        {
            AppUser? user = await _userManager.Users.Where(u => u.UserName == followToUsername).Include(u => u.Followers).FirstOrDefaultAsync();
            if (user is null) throw new AppUserNotFoundException($"User with username {followToUsername} didnt found!");

            AppUser? follower = await _userManager.Users.Where(u => u.UserName == followerUsername).Include(u => u.Follows).FirstOrDefaultAsync();
            if (follower is null) throw new AppUserNotFoundException($"User with username {followerUsername} didnt found!");

            if (follower.Follows.Any(fi => fi.UserName == user.UserName)) throw new AlreadyFollowedException($"You already followed to {followToUsername}!");
            follower.Follows.Add(new FollowItem
            {
                Name = user.Name,
                Surname = user.Surname,
                ImageUrl = user.ImageUrl,
                UserName = user.UserName,
                IsConfirmed = user.IsPrivate ? false : true
                    
            });
            user.Followers.Add(new FollowerItem
            {
                Name = follower.Name,
                Surname = follower.Surname,
                ImageUrl = follower.ImageUrl,
                IsConfirmed = user.IsPrivate ? false : true
            });

            await _userManager.UpdateAsync(user);
           
        }

        public async Task<AppUserGetDto> GetAsync(string username)
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user is null) throw new AppUserNotFoundException($"User with username {username} wasnt defined!");
            return _mapper.Map<AppUserGetDto>(user);
        }

        public async Task<CurrentAppUserGetDto> GetCurrentUserAsync(string username)
        {
            AppUser? user = await _userManager.Users
                .Include(u => u.LikedReplies)
                .Include(u => u.LikedPosts)
                    .ThenInclude(lp => lp.Post)
                .Include(u => u.LikedComments)
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (user is null) throw new AppUserNotFoundException($"User with username {username} wasnt defined!");
            CurrentAppUserGetDto dto =  _mapper.Map<CurrentAppUserGetDto>(user);
            dto.LikedCommentsIds = user.LikedComments.Select(cl => cl.CommentId).ToList();
            dto.LikedRepliesIds = user.LikedReplies.Select(lr => lr.ReplyId).ToList();
            return dto;
        }

        //auth methods
        public async Task<AppUserLoginResponseDto> LoginAsync(AppUserLoginDto dto)
        {
            AppUser user = await _userManager.FindByNameAsync(dto.UsernameOrEmail);
            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(dto.UsernameOrEmail);
                if (user is null) throw new AppUserNotFoundException("Username, email or password is incorrect!", 400);
            }
            if (await _userManager.IsLockedOutAsync(user)) throw new AppUserLockoutException("Too many failure attempts! Try later!");
            if (!await _userManager.CheckPasswordAsync(user, dto.Password)) throw new WrongPasswordException("Username, email or password is incorrect!");
            
            TokenResponseDto tokens = await _tokenService.GenerateTokensAsync(user, 15);
            user.RefreshToken = tokens.RefreshToken;
            user.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;
            await _userManager.UpdateAsync(user);
            return new AppUserLoginResponseDto(user.UserName, tokens.AccessToken, tokens.RefreshToken, tokens.RefreshTokenExpiresAt);


        }

        public async Task LogoutAsync(string refreshToken)
        {
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken
                                                                        && u.RefreshTokenExpiresAt > DateTime.UtcNow);
            if (user is not null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiresAt = null;
            }
        }

        public async Task<TokenResponseDto> RefreshAsync(string refreshToken)
        {
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken
                                                                        && u.RefreshTokenExpiresAt > DateTime.UtcNow);
            if (user is null) throw new InvalidTokenException("Token is not valid!");
            TokenResponseDto tokens = await _tokenService.GenerateTokensAsync(user, 15);
            user.RefreshToken = tokens.RefreshToken;
            user.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;
            await _userManager.UpdateAsync(user);
            return tokens;
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
            newUser.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;
            await _userManager.UpdateAsync(newUser);
            return new AppUserRegisterResponseDto(newUser.UserName, tokens.AccessToken, tokens.RefreshToken, tokens.RefreshTokenExpiresAt);
        }

    }
}
