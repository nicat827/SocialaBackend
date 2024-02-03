using Microsoft.AspNetCore.Http;
using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class Comment:BaseEntity
    {
        public string Text { get; set; } = null!;
        //relational 
        public int PostId { get; set; }
        public string Author { get; set; } = null!;
        public string? AuthorImageUrl { get; set; }
        //public int RepliesCount { get; set; }
        public int LikesCount { get; set; }
        public ICollection<CommentLikeItem> Likes { get; set; } = new List<CommentLikeItem>();
        public Post Post { get; set; } = null!;
        public ICollection<Reply> Replies { get; set; } = new List<Reply>();
    }
}
