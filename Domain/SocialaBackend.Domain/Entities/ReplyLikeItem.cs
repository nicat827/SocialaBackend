using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class ReplyLikeItem:BaseEntity
    {
        public int ReplyId { get; set; }
        public Reply Reply { get; set; } = null!;
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
