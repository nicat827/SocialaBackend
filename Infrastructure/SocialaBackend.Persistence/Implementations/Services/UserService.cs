using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.AppUsers;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Application.Exceptions.AppUser;
using SocialaBackend.Application.Exceptions.Forbidden;
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
        private readonly IHttpContextAccessor _http;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IFileService _fileService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly string _currentUserName;

        public UserService(IHttpContextAccessor http,ICloudinaryService cloudinaryService, IFileService fileService, UserManager<AppUser> userManager, IMapper mapper, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _http = http;
            _cloudinaryService = cloudinaryService;
            _currentUserName = http.HttpContext.User.Identity.Name;
            _fileService = fileService;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        public async Task ConfirmFollowerAsync(int id)
        {
            AppUser? user = await _userManager.Users.Where(u => u.UserName == _currentUserName).Include(u => u.Followers).FirstOrDefaultAsync();
            if (user is null) throw new AppUserNotFoundException($"User with _currentUserName {_currentUserName} doesnt exists!");
            FollowerItem? item = user.Followers.FirstOrDefault(f => f.IsConfirmed == false && f.Id == id);
            if (item is null) throw new NotFoundException($"Follower with id {id} wasnt defined!");

            AppUser? follower = await _userManager.Users.Where(u => u.UserName == item.UserName).Include(u => u.Follows).FirstOrDefaultAsync();
            if (follower is null) throw new AppUserNotFoundException($"Follower with username {_currentUserName} doesnt exists!");
            FollowItem? followItem = follower.Follows.FirstOrDefault(f => f.UserName == user.UserName);
            if (followItem is null) throw new NotFoundException($"Follow to {user.UserName} wasnt defined!");
            item.IsConfirmed = true;
            followItem.IsConfirmed = true;
            await _userManager.UpdateAsync(user);
            await _userManager.UpdateAsync(follower);
        }
        public async Task CancelFollowerAsync(string username)
        {
            AppUser? user = await _userManager.Users.Where(u => u.UserName == _currentUserName).Include(u => u.Followers).FirstOrDefaultAsync();
            if (user is null) throw new AppUserNotFoundException($"User with username {_currentUserName} doesnt exists!");
            FollowerItem? item = user.Followers.FirstOrDefault(f => f.UserName == username);
            if (item is null) throw new NotFoundException($"Follower  {username} wasnt defined!");

            AppUser? follower = await _userManager.Users.Where(u => u.UserName == item.UserName).Include(u => u.Follows).FirstOrDefaultAsync();
            if (follower is not null)
            {
                FollowItem? followItem = follower.Follows.FirstOrDefault(f => f.UserName == user.UserName);
                if (followItem is not null)
                {
                    follower.Follows.Remove(followItem);
                    await _userManager.UpdateAsync(follower);
                }
            }
            user.Followers.Remove(item);
            await _userManager.UpdateAsync(user);
        }
        public async Task CancelFollowAsync(string username)
        {
            AppUser? currentUser = await _userManager.Users.Where(u => u.UserName == _currentUserName).Include(u => u.Follows).FirstOrDefaultAsync();
            if (currentUser is null) throw new AppUserNotFoundException($"User with username {_currentUserName} doesnt exists!");
            FollowItem? item = currentUser.Follows.FirstOrDefault(f => f.UserName == username);
            if (item is null) throw new NotFoundException($"Follow didnt found!");
            AppUser? followingUser = await _userManager.Users.Where(u => u.UserName == item.UserName).Include(u => u.Followers).FirstOrDefaultAsync();
            if (followingUser is not null)
            {
                FollowerItem? followerItem = followingUser.Followers.FirstOrDefault(f => f.UserName == currentUser.UserName);
                if (followerItem is not null)
                {
                    followingUser.Followers.Remove(followerItem);
                    await _userManager.UpdateAsync(followingUser);
                }

            } 
            currentUser.Follows.Remove(item);
            await _userManager.UpdateAsync(currentUser);
        }
        public async Task FollowAsync(string followToUsername)
        {
            if (_currentUserName == followToUsername) throw new WrongFollowException("You cant follow to yourself!");
            AppUser? user = await _userManager.Users.Where(u => u.UserName == followToUsername).Include(u => u.Followers).FirstOrDefaultAsync();
            if (user is null) throw new AppUserNotFoundException($"User with username {followToUsername} didnt found!");

            AppUser? follower = await _userManager.Users.Where(u => u.UserName == _currentUserName).Include(u => u.Follows).FirstOrDefaultAsync();
            if (follower is null) throw new AppUserNotFoundException($"User with username {_currentUserName} didnt found!");

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
                UserName = follower.UserName,
                IsConfirmed = user.IsPrivate ? false : true
            });

            await _userManager.UpdateAsync(user);
           
        }

        public async Task<AppUserGetDto> GetAsync(string username)
        {
            AppUser? user = await _userManager.Users
                .Where(u => u.UserName == username)
                .Include(u => u.Followers.Where(f => f.IsConfirmed == true))
                .Include(u => u.Follows.Where(f => f.IsConfirmed == true))
                .FirstOrDefaultAsync();
            if (user is null) throw new AppUserNotFoundException($"User with username {username} wasnt defined!");
            AppUserGetDto dto = _mapper.Map<AppUserGetDto>(user);
            dto.FollowersCount = user.Followers.Count;
            dto.FollowsCount = user.Follows.Count;
            return dto;
        }

        public async Task<CurrentAppUserGetDto> GetCurrentUserAsync()
        {
            AppUser? user = await _userManager.Users
                .Include(u => u.Follows)
                .Include(u => u.Followers)
                .Include(u => u.LikedReplies)
                .Include(u => u.LikedPosts)
                    .ThenInclude(lp => lp.Post)
                .Include(u => u.LikedComments)
                .FirstOrDefaultAsync(u => u.UserName == _currentUserName);
            if (user is null) throw new AppUserNotFoundException($"User with _currentUserName {_currentUserName} wasnt defined!");
            CurrentAppUserGetDto dto =  _mapper.Map<CurrentAppUserGetDto>(user);
            dto.LikedCommentsIds = user.LikedComments.Select(cl => cl.CommentId).ToList();
            dto.LikedRepliesIds = user.LikedReplies.Select(lr => lr.ReplyId).ToList();
            return dto;
        }

        public async Task<ICollection<FollowGetDto>> GetFollowersAsync(string username, int? skip)
        {

            if (skip is null) skip = 0;
            AppUser? user = await _userManager.Users.Where(u => u.UserName == username).Include(u => u.Followers.Skip((int) skip)).Take(10).FirstOrDefaultAsync();
            if (user is null) throw new AppUserNotFoundException($"User with username {username} doesnt exists!");
            if (user.IsPrivate)
                {
                    if (username != _currentUserName)
                        if (!user.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                            throw new ForbiddenException("This account is private, follow for seeing followers!");
                }
            return _mapper.Map<ICollection<FollowGetDto>>(user.Followers);
        }

        public async Task<ICollection<FollowGetDto>> GetFollowsAsync(string username, int? skip)
        {
            if (skip is null) skip = 0;
            AppUser? user = await _userManager.Users.Where(u => u.UserName == username).Include(u => u.Followers).Include(u => u.Follows.Skip((int)skip)).Take(10).FirstOrDefaultAsync();
            if (user is null) throw new AppUserNotFoundException($"User with username {username} doesnt exists!");
            if (user.IsPrivate)
            {
                if (username != _currentUserName)
                    if (!user.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                        throw new ForbiddenException("This account is private, follow for seeing followers!");
            }
            return _mapper.Map<ICollection<FollowGetDto>>(user.Follows);
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
            
            var res = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, true);
            
            if (res.IsLockedOut)
            {
                TimeSpan blockTimeLeft = (DateTimeOffset)(user.LockoutEnd) - DateTimeOffset.UtcNow;
                byte totalMinutes = (byte)blockTimeLeft.Minutes;
                byte totalSeconds = (byte)blockTimeLeft.Seconds;
                throw new AppUserLockoutException($"Too many failure attempts! Try later!",totalMinutes, totalSeconds);
            }
            if (!res.Succeeded) throw new WrongPasswordException("Username, email or password is incorrect!");

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

            if (await _userManager.Users.AnyAsync(u => u.UserName == dto.Username)) throw new UserAlreadyExistException($"User with _currentUserName {dto.Username} already exists!");
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
            if (dto.Photo is not null)
            {
                string imageUrl = await _fileService.CreateFileAsync(dto.Photo, "uploads", "users", "avatars");
                string cloudinaryUrl = await _cloudinaryService.UploadFileAsync(imageUrl, "uploads", "users", "avatars");
                newUser.ImageUrl = cloudinaryUrl;
            }
            await _userManager.AddToRoleAsync(newUser, UserRole.Member.ToString());

            TokenResponseDto tokens = await _tokenService.GenerateTokensAsync(newUser, 15);
            newUser.RefreshToken = tokens.RefreshToken;
            newUser.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;
            await _userManager.UpdateAsync(newUser);
            return new AppUserRegisterResponseDto(newUser.UserName, tokens.AccessToken, tokens.RefreshToken, tokens.RefreshTokenExpiresAt);
        }

    }
}
