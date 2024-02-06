using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Enums;
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
        public string Email { get; set; } = null!;

        public string? Bio { get; set; }

        public string? FacebookLink { get; set; }
        public string? GithubLink { get; set; }
        public string? InstagramLink { get; set; }

        public Gender Gender { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPrivate { get; set; }
        public ICollection<FollowGetDto> Follows { get; set; } = new List<FollowGetDto>();
        public ICollection<FollowGetDto> Followers { get; set; } = new List<FollowGetDto>();
        public ICollection<AppUserLikesGetDto> LikedPosts { get; set; } = new List<AppUserLikesGetDto>();
        public IEnumerable<int> LikedCommentsIds { get; set; } = new List<int>();
        public IEnumerable<int> LikedRepliesIds { get; set; } = new List<int>();
    }
}
