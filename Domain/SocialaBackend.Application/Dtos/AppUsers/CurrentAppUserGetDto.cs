using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos.AppUsers
{
    public class CurrentAppUserGetDto {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<AppUserLikesGetDto> LikedPosts { get; set; } = new List<AppUserLikesGetDto>();
        public ICollection<int> LikedCommentsIds { get; set; } = new List<int>();
    }
}
