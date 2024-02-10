using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class AppUserResetPasswordDto
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Token { get; set; } = null!;
    }
}
