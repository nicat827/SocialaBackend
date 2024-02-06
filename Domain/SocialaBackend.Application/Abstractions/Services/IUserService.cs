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
        Task RegisterAsync(AppUserRegisterDto dto);
        Task<AppUserRegisterResponseDto> ConfirmEmailAsync(string token, string email);

        Task<AppUserLoginResponseDto> LoginAsync(AppUserLoginDto dto);

        Task<TokenResponseDto> RefreshAsync(string refreshToken);

        Task LogoutAsync(string refreshToken);

        Task<AppUserGetDto> GetAsync(string username);
        Task<CurrentAppUserGetDto> GetCurrentUserAsync();

        //follow methods
        Task FollowAsync(string followToUsername);
        Task ConfirmFollowerAsync(int id);
        Task CancelFollowerAsync(string username);
        Task CancelFollowAsync(string username);
        Task<ICollection<FollowGetDto>> GetFollowersAsync(string username,int? skip);
        Task<ICollection<FollowGetDto>> GetFollowsAsync(string username,int? skip);


    }

}
