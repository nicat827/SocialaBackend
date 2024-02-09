using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class PostLikeItem:BaseEntity
    {
       
        public string? LikedUserId { get; set; }
        public AppUser? LikedUser { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
       
    }
}
