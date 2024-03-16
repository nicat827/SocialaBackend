using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class CheckedUserGetDto
    {
        public string UserName { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
