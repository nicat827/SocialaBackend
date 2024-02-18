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
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int StoryId { get; set; }
        public string? Bio { get; set; }

        

        public string? FacebookLink { get; set; }
        public string? GithubLink { get; set; }
        public string? InstagramLink { get; set; }

        public Gender Gender { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public string? ImageUrl { get; set; }
        public bool IsPrivate { get; set; }

        public DateTime? LastStoryPostedAt { get; set; }

        public string? BackgroundImage { get; set; }
        public ICollection<NotificationsGetDto> Notifications { get; set; } = new List<NotificationsGetDto>();
        public IEnumerable<string> LikedAvatarsUsernames { get; set; } = new List<string>();
        public ICollection<FollowGetDto> Follows { get; set; } = new List<FollowGetDto>();
        public ICollection<FollowGetDto> Followers { get; set; } = new List<FollowGetDto>();
        public ICollection<int> LikedPostsIds { get; set; } = new List<int>();
        public IEnumerable<int> LikedCommentsIds { get; set; } = new List<int>();
        public IEnumerable<int> LikedRepliesIds { get; set; } = new List<int>();
        public IEnumerable<int> WatchedStoryItemsIds { get; set; } = new List<int>();
    }
}
