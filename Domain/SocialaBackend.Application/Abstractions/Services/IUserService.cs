using SocialaBackend.Application.Dtos.Tokens;
using SocialaBackend.Application.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<TokenResponseDto> RegisterAsync(UserPostDto dto);
    }
}
