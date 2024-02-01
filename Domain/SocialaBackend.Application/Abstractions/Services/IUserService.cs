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

        Task<AppUserGetDto> GetAsync(string username);
    }
}
