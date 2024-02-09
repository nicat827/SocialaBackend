using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class Reply:BaseEntity
    {
        public string Text { get; set; } = null!;
        public int LikesCount { get; set; }
        //relational 
        public IList<ReplyLikeItem> Likes { get; set; } = new List<ReplyLikeItem>();

        public int CommentId { get; set; }

        public Comment Comment { get; set; } = null!;

        public string? AuthorId { get; set; }
        public AppUser? Author { get; set; } 
    }
}
