using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos.AppUsers
{
    public record AppUserLoginDto(string UsernameOrEmail, string Password, bool IsPersistence);

}
