using SocialaBackend.Domain.Enums;
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
        public string? FacebookLink { get; set; }
        public string? GithubLink { get; set; }
        public string? InstagramLink { get; set; }
        public Gender Gender { get; set; }
        public string? ImageUrl { get; set; }
        public string? BackgroundImage { get; set; }
        public bool IsPrivate { get; set; }
        public int FollowsCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowsRequestCount { get; set; }
        public int FollowerRequestCount { get; set; }
    }
    
}
