using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.AppUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<AppUserRegisterResponseDto> RegisterAsync(AppUserRegisterDto dto);

        Task<AppUserLoginResponseDto> LoginAsync(AppUserLoginDto dto);

        Task<TokenResponseDto> RefreshAsync(string refreshToken);

        Task LogoutAsync(string refreshToken);

        Task<AppUserGetDto> GetAsync(string username);
        Task<CurrentAppUserGetDto> GetCurrentUserAsync(string username);

        //follow methods
        Task FollowAsync(string followerUsername, string followToUsername);
        Task ConfirmFollowerAsync(string username, int id);
        Task CancelFollowerAsync(string username, int id);
        Task CancelFollowAsync(string username, int id);
        Task<ICollection<FollowGetDto>> GetFollowersAsync(string username, int? skip);
        Task<ICollection<FollowGetDto>> GetFollowsAsync(string username, int? skip);


    }

}
