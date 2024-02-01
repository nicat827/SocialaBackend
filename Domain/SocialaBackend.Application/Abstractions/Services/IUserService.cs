using SocialaBackend.Application.Dtos;
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

        Task<AppUserGetDto> GetAsync(string username);
    }
}
