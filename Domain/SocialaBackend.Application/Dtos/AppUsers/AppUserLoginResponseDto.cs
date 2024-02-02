using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public record AppUserLoginResponseDto(string Username, string AccessToken, string RefreshToken, DateTime ExpiresAt);

}
