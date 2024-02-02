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
        public string AppUserId { get; set; } = null!;

        public AppUser AppUser { get; set; } = null!;

        public int? PostId { get; set; }

        public Post? Post { get; set; } = null!;
       
    }
}
