using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class AppUserConfirmEmailDto
    {
        public string Token { get; set; } = null!;

        public string Email { get; set; } = null!;
           
    }
}
