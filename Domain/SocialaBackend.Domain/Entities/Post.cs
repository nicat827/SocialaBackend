using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class Post:BaseEntity
    {
        public string SourceUrl { get; set; } = null!;
        public string? Description { get; set; }

        //relational
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<PostLikeItem> Likes { get; set; } = new List<PostLikeItem>();
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;


    }
}
