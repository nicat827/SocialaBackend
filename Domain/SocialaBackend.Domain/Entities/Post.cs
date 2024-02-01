using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class Post:BaseNameableEntity
    {
        public string SourceUrl { get; set; } = null!;
        public string? Description { get; set; }

        //relational
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<PostLikeItem>? Likes { get; set; }
        public string ApppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;


    }
}
