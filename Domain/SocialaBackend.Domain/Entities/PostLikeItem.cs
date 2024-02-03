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
        public string Username { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? AppUserId { get; set; } = null!;
        public AppUser? AppUser { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
       
    }
}
