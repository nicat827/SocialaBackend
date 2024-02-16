using SocialaBackend.Application.Dtos;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface ITokenService
    {
        Task<TokenResponseDto> GenerateTokensAsync(AppUser user, bool isPersistence);
    }
}
