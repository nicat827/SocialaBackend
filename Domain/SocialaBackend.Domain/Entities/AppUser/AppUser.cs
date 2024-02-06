using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities.User
{
    public class AppUser:IdentityUser
    {
        
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? Bio { get; set; }

        public Gender Gender { get; set; }

        public bool IsPrivate { get; set; }

        public string? BackgroundImage { get; set; }    
        public string? FeacebookLink { get; set; }    
        public string? GithubLink { get; set; }    
        public string? InstagramLink { get; set; }

        public bool PhotoLikeNotify { get; set; } = true;
        public bool PostLikeNotify { get; set; } = true;
        public bool FollowerNotify { get; set; } = true;

        public string? RefreshToken { get; set; } = null!;

        public DateTime? RefreshTokenExpiresAt { get; set; }

        //relational
        public ICollection<FollowItem> Follows { get; set; } = new List<FollowItem>();
        public ICollection<FollowerItem> Followers { get; set; } = new List<FollowerItem>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<PostLikeItem> LikedPosts { get; set; } = new List<PostLikeItem>();
        public ICollection<CommentLikeItem> LikedComments { get; set; } = new List<CommentLikeItem>();
        public ICollection<ReplyLikeItem> LikedReplies { get; set; } = new List<ReplyLikeItem>();
    }
}
