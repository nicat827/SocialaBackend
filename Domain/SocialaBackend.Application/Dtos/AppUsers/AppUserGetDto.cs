using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class AppUserGetDto
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }
        public string? BackgroundImage { get; set; }
        public bool IsPrivate { get; set; }
        public int FollowsCount { get; set; }
        public int FollowersCount { get; set; }
    }
    
}
