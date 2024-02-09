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
       
        public int RepliesCount { get; set; }
        public int LikesCount { get; set; }
        //relational 
        public string? AuthorId { get; set; } = null!;
        public AppUser? Author { get; set; } = null!;
        public IList<CommentLikeItem> Likes { get; set; } = new List<CommentLikeItem>();
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
        public ICollection<Reply> Replies { get; set; } = new List<Reply>();
    }
}
