using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class FollowItem:BaseEntity
    {
        public string UserName { get; set; } = null!;

        public bool IsConfirmed { get; set; }

        public string? ImageUrl { get; set; }

        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        //relational

        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;

        
    }
}
